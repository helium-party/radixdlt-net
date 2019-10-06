using HeliumParty.RadixDLT.Exceptions;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Primitives;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HeliumParty.RadixDLT.Pow
{
    public class ProofOfWork
    {
        private readonly int _magic;
        private readonly byte[] _seed;
        private readonly byte[] _target;

        public long Nonce { get; }
        public string TargetHex => Bytes.ToHexString(_target);
        

        public ProofOfWork(long nonce, int magic, byte[] seed, byte[] target)
        {
            Nonce = nonce;
            _magic = magic;
            _seed = seed;
            _target = target;
        }

        public void Validate()
        {
            using (var stream = new MemoryStream(4 + 32 + 8))
            {
                stream.Write(BitConverter.GetBytes(_magic), 0, 4);
                stream.Write(_seed, 0, _seed.Length);
                stream.Write(BitConverter.GetBytes(Nonce), 0, 8);
                var hashHex = Bytes.ToHexString(RadixHash.From(stream.ToArray()).ToByteArray());

                if (hashHex.CompareTo(TargetHex) > 0)
                    throw new ProofOfWorkException(hashHex, TargetHex);
            }

        }

        public override string ToString()
        {
            return $"POW: nonce({Nonce}) magic({_magic}) seed({Base64.ToBase64String(_seed)}) target({TargetHex})";
        }
    }
}
