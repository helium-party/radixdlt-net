using Newtonsoft.Json.Linq;

namespace HeliumParty.RadixDLT.Atoms
{
    /// <summary>
    /// An event where an atom's status has changed
    /// </summary>
    public class AtomStatusEvent
    {
        public AtomStatus Status { get; }
        public JObject Data { get; }

        public AtomStatusEvent(AtomStatus status, JObject data)
        {
            Status = status;
            Data = data;
        }

        public AtomStatusEvent(AtomStatus status) : this(status, null) { }

        public override string ToString() => $"{Status} {Data}";
    }
}
