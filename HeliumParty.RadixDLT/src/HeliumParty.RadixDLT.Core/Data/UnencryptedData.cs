using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Data
{
    public class UnencryptedData
    {
        public byte[] Data { get; }
        public Dictionary<string, object> MetaData { get; }

        public UnencryptedData(byte[] data, Dictionary<string, object> metaData)
        {
            Data = data;
            MetaData = metaData;
        }
    }
}
