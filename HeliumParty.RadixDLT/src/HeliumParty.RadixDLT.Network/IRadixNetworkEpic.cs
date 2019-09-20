using System;
using System.Reactive;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// Inspired by epics in redux-observables. This follows a similar pattern which 
    /// can be understood as a creator of a stream of actions from a stream of actions.
    /// </summary>
    public interface IRadixNetworkEpic
    {
        /// <summary>
        /// Creates a stream of actions from a stream of actions and current/future states.
        /// 
        /// Note that this should NEVER let an action "slip through" (emit action which it has received)
        /// as it will cause an infinite loop.
        /// </summary>
        /// <param name="actions">Stream of actions comming in</param>
        /// <param name="networkState">Stream of states comming in</param>
        /// <returns>Stream of new actions</returns>
        IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState);
    }
}
