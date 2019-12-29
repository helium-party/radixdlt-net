using HeliumParty.RadixDLT.Jsonrpc;
using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which submits an atom to a node over websocket and 
    /// emits events as the atom is accepted by the network
    /// </summary>
    public class SubmitAtomEpic : IRadixNetworkEpic
    {
        private static readonly TimeSpan DelayClose = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

        private readonly WebSockets _WebSockets;

        public SubmitAtomEpic(WebSockets webSockets)
        {
            _WebSockets = webSockets;
        }

        /// <summary>
        /// Observable to only wait until node is connected
        /// Note that the return collection won't provide any items
        /// </summary>
        private IObservable<IRadixNodeAction> WaitForConnection(RadixNode node)
        {
            return Utils.WaitForConnection(_WebSockets, node, out var ws)
                .FirstAsync()
                .Select(_ => default(IRadixNodeAction))
                .IgnoreElements();
        }

        private IObservable<IRadixNodeAction> SubmitAtom(Actions.SubmitAtomSendAction request, RadixNode node)
        {
            var ws = _WebSockets.GetOrCreate(node);
            var client = new RadixJsonRpcClient(ws);
            var subscriberId = new System.Guid().ToString();

            return client.ObserveAtomStatusNotifications(subscriberId)
                .SelectMany(notification =>
                {
                    if (notification.NotificationType.Equals(JsonRpcNotificationType.Start))
                        return client.RequestAtomStatusNotifications<IRadixNodeAction>(subscriberId, request.AtomSubmission.Id)
                        .Concat(client.PushAtom<IRadixNodeAction>(request.AtomSubmission))
                        .Concat(Observable.Return(new Actions.SubmitAtomReceivedAction(request.Id, request.AtomSubmission, node)))
                        .Catch<IRadixNodeAction, SubmitAtomException>(exc =>
                        {
                            return Observable.Concat<IRadixNodeAction>(
                                    Observable.Return<IRadixNodeAction>(
                                        new Actions.SubmitAtomStatusAction(
                                            request.Id,
                                            request.AtomSubmission,
                                            node,
                                            new Atoms.AtomStatusEvent(Atoms.AtomStatus.EvictedInvalidAtom, exc.Error)
                                            )),
                                    Observable.Return<IRadixNodeAction>(
                                        new Actions.SubmitAtomCompleteAction(request.Id, request.AtomSubmission, node)));
                        });
                    else
                    {
                        var statusNotification = notification.NotificationEvent;
                        var statusEvent = new Actions.SubmitAtomStatusAction(
                            request.Id,
                            request.AtomSubmission,
                            node,
                            statusNotification);

                        if (statusNotification.Status == Atoms.AtomStatus.Stored || !request.IsCompleteOnStoreOnly)
                            return Observable.Concat<IRadixNodeAction>(
                                Observable.Return(statusEvent),
                                Observable.Return(new Actions.SubmitAtomCompleteAction(request.Id, request.AtomSubmission, node)));
                        else
                            return Observable.Return(statusEvent);
                    }
                })
                .Finally(() =>
                {
                    client.CloseAtomStatusNotifications<object>(subscriberId)
                    .Concat(
                        Observable.Timer(DelayClose)
                        .SelectMany(t =>
                        {
                            ws.Close();
                            return Observable.Empty<object>();
                        })
                        ).Subscribe();
                })
                .Timeout(Timeout, Observable.Return(new Actions.SubmitAtomCompleteAction(request.Id, request.AtomSubmission, node)))
                .TakeUntil(a => a is Actions.SubmitAtomCompleteAction)
                .Finally(() => Observable.Timer(DelayClose).Subscribe(_ => ws.Close()));
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            var foundNode = actions
                .OfType<Actions.FindANodeResultAction>()
                .Where(a => a.Request is Actions.SubmitAtomRequestAction)
                .Select(a =>
                {
                    var request = (Actions.SubmitAtomRequestAction)a.Request;
                    return new Actions.SubmitAtomSendAction(request.Id, request.AtomSubmission, a.Node, request.IsCompleteOnStoreOnly);
                });

            var submitToNode = actions
                .OfType<Actions.SubmitAtomSendAction>()
                .SelectMany(a => this.WaitForConnection(a.Node).Concat(this.SubmitAtom(a, a.Node)));

            return Observable.Merge(foundNode, submitToNode);
        }
    }
}
