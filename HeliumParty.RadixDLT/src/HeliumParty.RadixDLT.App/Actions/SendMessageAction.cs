using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Actions
{
    public class SendMessageAction : IAction
    {
        public RadixAddress From { get; }
        public RadixAddress To { get; }
        public byte[] Data { get; }
        public bool Encrypt { get;  }

        public SendMessageAction(RadixAddress from, RadixAddress to, byte[] data, bool encrypt)
        {
            From = from;
            To = to;
            Data = data;
            Encrypt = encrypt;
        }

        public override string ToString()
        {
            return $"SEND MESSAGE FROM {From} TO {To}";
        }
    }
}
