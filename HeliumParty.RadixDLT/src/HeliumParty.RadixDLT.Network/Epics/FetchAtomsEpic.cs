using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using HeliumParty.RadixDLT.Jsonrpc;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which emits atoms on a <see cref="Actions.FetchAtomsRequestAction"/> query forever until a <see cref="Actions.FetchAtomsCancelAction"/> occurs
    /// </summary>
    public class FetchAtomsEpic : IRadixNetworkEpic
    {
        private static readonly TimeSpan DelayClose = TimeSpan.FromMinutes(5);
        private readonly WebSockets _WebSockets;

        public FetchAtomsEpic(WebSockets webSockets)
        {
            _WebSockets = webSockets;
        }

        /// <summary>
        /// Observable to only wait until node is connected
        /// Note that the return collection won't (and shouldn't have to) provide any items
        /// </summary>
        private IObservable<IRadixNodeAction> WaitForConnection(RadixNode node)
        {
            return Utils.WaitForConnection(_WebSockets, node, out var ws)
                .FirstAsync()
                .Select(_ => default(IRadixNodeAction))
                .IgnoreElements();
        }

        private IObservable<IRadixNodeAction> FetchAtoms(Actions.FetchAtomsRequestAction request, RadixNode node)
        {
            var ws = _WebSockets.GetOrCreate(node);
            var address = request.Address;
            var client = new RadixJsonRpcClient(ws);

            return client.ObserveAtoms(request.Id)
                .SelectMany(n =>
                {
                    if (n.NotificationType == JsonRpcNotificationType.Start)
                        return client.SendAtomsSubscribe<IRadixNodeAction>(request.Id, address);
                    else
                        return Observable.Return(new Actions.FetchAtomsObservationAction(request.Id, address, node, n.NotificationEvent));
                })
                .Finally<IRadixNodeAction>(() =>
                {
                    client.CancelAtomsSubscribe<IRadixNodeAction>(request.Id)
                    .Concat<IRadixNodeAction>(
                        Observable.Timer(DelayClose)
                        .SelectMany(_ =>
                        {
                            ws.Close();
                            return Observable.Empty<IRadixNodeAction>();
                        })
                        ).Subscribe();
                })
                .StartWith(new Actions.FetchAtomsSubscribeAction(request.Id, address, node));
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            var disposables = new ConcurrentDictionary<string, IDisposable>();

            var cancelFetch = actions
                .OfType<Actions.FetchAtomsCancelAction>()
                .Do(a =>
                {
                    if (disposables.TryRemove(a.Id, out var disposable))
                        disposable?.Dispose();
                })
                .IgnoreElements();

            var fetch = actions
                .OfType<Actions.FindANodeResultAction>()
                .Where(a => a.Request is Actions.FetchAtomsRequestAction)
                .SelectMany(a =>
                {
                    var request = (Actions.FetchAtomsRequestAction)a.Request;

                    return Observable.Defer<IRadixNodeAction>(() =>
                    {
                        // TODO: !!! Enable disposing of the following action via the 'FetchAtomsCancelAction' above
                        // disposables.TryAdd(request.Id, )

                        return WaitForConnection(a.Node)
                            .Concat<IRadixNodeAction>(FetchAtoms(request, a.Node));
                    });
                });

            return Observable.Merge(cancelFetch, fetch);
        }
    }
}
