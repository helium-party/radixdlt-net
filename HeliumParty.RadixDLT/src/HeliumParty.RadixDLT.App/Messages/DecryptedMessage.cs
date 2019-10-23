using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Messages
{
    public class DecryptedMessage
    {
        public RadixAddress From { get; }
        public RadixAddress To { get; }
        public byte[] Data { get; }
        public MessageEncryptionState EncryptionState { get; }
        public long TimeStamp { get;  }
        public EUID ActionId { get;  }

        public DecryptedMessage(RadixAddress from, RadixAddress to, byte[] data, MessageEncryptionState encryptionState, long timeStamp, EUID actionId)
        {
            From = from;
            To = to;
            Data = data;
            EncryptionState = encryptionState;
            TimeStamp = timeStamp;
            ActionId = actionId;
        }

        public override string ToString()
        {
            return $"timestamp {From} -> {To}: {EncryptionState} {Convert.ToBase64String(Data)}";
        }
    }
}
