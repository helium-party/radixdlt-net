using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Mappers
{
    public interface IAtomErrorToExceptionReasonMapper
    {
        IEnumerable<ActionExecutionExceptionReason> MapAtomErrorToExceptionReasons(Atom atom, string errorData);
    }
}
