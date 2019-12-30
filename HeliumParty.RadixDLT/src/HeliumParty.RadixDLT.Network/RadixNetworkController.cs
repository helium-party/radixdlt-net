using HeliumParty.RadixDLT.Reducers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// The meat and bones of the networking module. This module connects all the epics 
    /// and reducers to produce a stream of actions and states
    /// </summary>
    public class RadixNetworkController
    {
        private static readonly Log.OutputLogger _Logger = new Log.OutputLogger();
        private readonly SubjectBase<IRadixNodeAction> _NodeActions = new ReplaySubject<IRadixNodeAction>();
        
        public BehaviorSubject<RadixNetworkState> NetworkState { get; }

        /// <summary>
        /// Observable of all actions whichi have occured in the network system.
        /// Actions are only emitted after they have been processed by all reducers.
        /// </summary>
        public IObservable<IRadixNodeAction> ReducedNodeActions { get; }

        // TODO: Might need to adjust to java lib, if changes are made
        
        internal RadixNetworkController(
            RadixNetwork network, 
            RadixNetworkState initialState,
            ImmutableList<IRadixNetworkEpic> epics,
            ImmutableList<Action<IRadixNodeAction>> reducers)
        {
            if (network == null || epics == null || reducers == null) 
                throw new System.ArgumentNullException( network == null ? nameof(network) : 
                    epics == null ? nameof(epics) : nameof(reducers));

            NetworkState = new BehaviorSubject<RadixNetworkState>(initialState);
            
            IConnectableObservable<IRadixNodeAction> connectableReducedNodeActions = System.Reactive.Linq.Observable.Do(
                _NodeActions,
                action =>
                {
                    RadixNetworkState curState = NetworkState.Value;
                    RadixNetworkState nextState = network.Reduce(curState, action);
                    reducers.ForEach(r => r.Invoke(action));

                    if (_Logger.IsDebugEnabled)
                        _Logger.LogMessage(action.ToString(), Log.LogLevel.Debug);

                    if (nextState.Equals(curState))
                        NetworkState.OnNext(nextState);
                }).Publish();

            // Execute each epic
            HashSet<IObservable<IRadixNodeAction>> updates = new HashSet<IObservable<IRadixNodeAction>>();
            foreach (var epic in epics)
                updates.Add(epic.Epic(connectableReducedNodeActions, NetworkState));

            // TODO: Cleanup the returned disposable
            Observable.Merge(updates).Subscribe(
                this.Dispatch,
                e =>
                {
                    _Logger.LogMessage(e.Message, Log.LogLevel.Critical);
                    NetworkState.OnError(e);
                });

            ReducedNodeActions = connectableReducedNodeActions;
            connectableReducedNodeActions.Connect();
        }
        
        /// <summary>
        /// Dispatches an action into the system. That is, it will be processed through reducers and then subsequently epics
        /// </summary>
        /// <param name="action">The action to dispatch</param>
        public void Dispatch(IRadixNodeAction action)
        {
            _NodeActions.OnNext(action);
        }
    }
}
