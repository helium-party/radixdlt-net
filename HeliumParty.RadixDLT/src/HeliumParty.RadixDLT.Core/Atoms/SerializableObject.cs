using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Atoms
{
    public abstract class SerializableObject
    {
        public int Version { get; set; }

        public SerializableObject()
        {
            Version = 100;
        }
    }
}
