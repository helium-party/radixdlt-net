using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public static class RadixRpcUtils
    {
        /// <summary>
        /// Checks whether a call was successful, in case it was not, an exeption will be thrown
        /// </summary>
        /// <typeparam name="T">Type of returned observable as the termination message alone is returned</typeparam>
        /// <param name="response">The response to check for success</param>
        /// <returns>An <see cref="IObservable{T}"/> that only leaves the termination message when message call was accepted</returns>
        /// <exception cref="JsonRpcCallException">In case the call was not successful</exception>
        public static IObservable<T> CheckCallSuccess<T>(this IObservable<JsonRpcResponse> response)
        {
            return response.Select(r =>
            {
                if (!r.WasSuccessful)
                    throw new JsonRpcCallException();
                return r;
            }
            )
            .Select(_ => default(T))
            .IgnoreElements();
        }
    }
}
