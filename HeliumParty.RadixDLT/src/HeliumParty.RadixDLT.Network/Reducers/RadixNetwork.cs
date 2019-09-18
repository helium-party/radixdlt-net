using HeliumParty.RadixDLT.Actions;
using System.Collections.Generic;

namespace HeliumParty.RadixDLT.Reducers
{
    /// <summary>
    /// Reducer which controls state transitions as new network actions occur.
    /// Explicitly does not contain state and should be maintained as a pure function.
    /// </summary>
    public class RadixNetwork // TODO: Can we make it static?
    {
        // TODO - Refactor?
        public RadixNetworkState Reduce(RadixNetworkState state, IRadixNodeAction action)
        {
            Dictionary<RadixNode, RadixNodeState> tmpDic = null;

            if (action is GetNodeDataResultAction ndr)
            {
                tmpDic = new Dictionary<RadixNode, RadixNodeState>(state.NodeStateCollection);

                var value = RadixNodeState.From(
                    ndr.Node,
                    Web.WebSocketStatus.Connected,
                    ndr.GetResult());

                if (tmpDic.ContainsKey(ndr.Node))
                {
                    var oldValue = tmpDic[ndr.Node];
                    if (oldValue == null)
                        tmpDic[ndr.Node] = value;

                    else
                        // Only override the data property
                        tmpDic[ndr.Node] = RadixNodeState.From(
                            oldValue.GetNode(), 
                            oldValue.Status, 
                            value.GetData(), 
                            oldValue.UniverseConfig);
                }
            }

            else if (action is GetUniverseResponseAction ur)
            {
                tmpDic = new Dictionary<RadixNode, RadixNodeState>(state.NodeStateCollection);

                var value = RadixNodeState.From(
                    ur.Node,
                    Web.WebSocketStatus.Connected,
                    null,
                    ur.GetResult());

                if (tmpDic.ContainsKey(ur.Node))
                {
                    var oldValue = tmpDic[ur.Node];
                    if (oldValue == null)
                        tmpDic[ur.Node] = value;

                    else
                        tmpDic[ur.Node] = RadixNodeState.From(
                            oldValue.GetNode(),
                            oldValue.Status,
                            oldValue.GetData(),
                            value.UniverseConfig
                            );
                }
            }

            else if (action is AddNodeAction an)
            {
                tmpDic = new Dictionary<RadixNode, RadixNodeState>(state.NodeStateCollection);

                var value = RadixNodeState.From(
                    an.Node,
                    Web.WebSocketStatus.Disconnected,
                    an.Data
                    );

                if (tmpDic.ContainsKey(an.Node))
                {
                    var oldValue = tmpDic[an.Node];
                    if (oldValue == null)
                        tmpDic[an.Node] = value;

                    else
                        tmpDic[an.Node] = RadixNodeState.From(
                            oldValue.GetNode(),
                            value.Status,
                            value.GetData() ?? oldValue.GetData(),
                            oldValue.UniverseConfig
                            );
                }
            }

            else if (action is WebSocketEvent ws)
            {
                tmpDic = new Dictionary<RadixNode, RadixNodeState>(state.NodeStateCollection);

                // Convert WebSocketEventType enum to WebSocketStatus enum
                System.Enum.TryParse<Web.WebSocketStatus>(ws.Type.ToString(), out var convStatus);      // TODO: Works, but we should create a unit test for it

                var value = RadixNodeState.From(
                    ws.Node,
                    convStatus
                    );

                if (tmpDic.ContainsKey(ws.Node))
                {
                    var oldValue = tmpDic[ws.Node];
                    if (oldValue == null)
                        tmpDic[ws.Node] = value;

                    else
                        tmpDic[ws.Node] = RadixNodeState.From(
                            oldValue.GetNode(),
                            value.Status,
                            value.GetData() ?? oldValue.GetData(),
                            oldValue.UniverseConfig
                            );
                }
            }

            return (tmpDic != null) ? new RadixNetworkState(tmpDic) : state;
        }
    }
}
