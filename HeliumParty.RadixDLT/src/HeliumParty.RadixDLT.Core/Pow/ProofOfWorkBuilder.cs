
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HeliumParty.RadixDLT.Pow
{
    public class ProofOfWorkBuilder
    {
        public Task<ProofOfWork> Build(int magic, byte[] seed, int leading)
        {
            return new Task<ProofOfWork>(() => 
            {
                if (seed.Length != 32 || leading < 1 || leading > 256)
                    throw new ArgumentException();

                BitArray targetBitSet = new BitArray(256);

                targetBitSet.SetAll(true);

                for (int i = 0; i < leading; i++)
                    targetBitSet.Set(i, false);

                for (int i = leading + (8 - leading % 8); i < leading + 8; i++)
                    targetBitSet.Set(i, false);

                byte[] target = new byte[targetBitSet.Length / 8];
                targetBitSet.CopyTo(target, 0);

                using (var stream = new MemoryStream(32 + 4 + 8))
                {
                    var nonce = 1L;
                    var magicbytes = BitConverter.GetBytes(magic);
                    var targetHex = Bytes.ToHexString(target);

                    stream.Write(magicbytes, 0, magicbytes.Length);
                    stream.Write(seed, 0, seed.Length);

                    while (true)
                    {
                        stream.Position = 32 + 4;
                        var nonceBytes = BitConverter.GetBytes(nonce);
                        stream.Write(nonceBytes, 0, nonceBytes.Length);
                        var hashHex = Bytes.ToHexString(RadixHash.From(stream.ToArray()).ToByteArray());
                        if (hashHex.CompareTo(targetHex) < 0)
                            return new ProofOfWork(nonce, magic, seed, target);

                        nonce++;
                    }
                }
            });           
        }
    }
}
