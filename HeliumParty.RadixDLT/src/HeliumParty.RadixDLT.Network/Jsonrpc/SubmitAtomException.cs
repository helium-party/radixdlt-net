using HeliumParty.RadixDLT.Atoms;
using Newtonsoft.Json.Linq;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class SubmitAtomException : System.Exception
    {
        public Atom FailedSubmissionAtom { get; }
        public JObject Error { get; }

        public SubmitAtomException(Atom atom, JObject error) : base()
        {
            FailedSubmissionAtom = atom;
            Error = error;
        }
    }
}
