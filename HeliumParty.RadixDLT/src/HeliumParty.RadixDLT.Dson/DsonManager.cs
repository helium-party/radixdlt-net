using System.IO;
using System.Threading.Tasks;
using Dahomey.Cbor;
using Dahomey.Cbor.Serialization.Converters;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Mapping;

namespace HeliumParty.RadixDLT
{
    public class DsonManager
    {
        static DsonManager()
        {
            // registering custom dson converters
            CborConverter.Register(typeof(byte[]), new DsonObjectConverter<byte[]>(x => x, y => y));
            CborConverter.Register(typeof(EUID), new DsonObjectConverter<EUID>(x => x.ToByteArray(), y => new EUID(y)));
            //CborConverter.Register(typeof(Hash), new DsonObjectConverter<Hash>(x => x.ToByteArray(), y => new Hash(y)));
            CborConverter.Register(typeof(RadixAddress), new DsonObjectConverter<RadixAddress>(x => Base58Encoding.Decode(x.ToString()), y => new RadixAddress(Base58Encoding.Encode(y))));
            //CborConverter.Register(typeof(UInt256), new DsonObjectConverter<UInt256>(x => x.ToByteArray(), UInt256.From));
            CborConverter.Register(typeof(RRI), new DsonObjectConverter<RRI>(x => RadixConstants.StandardEncoding.GetBytes(x.ToString()), y => new RRI(RadixConstants.StandardEncoding.GetString(y))));
            //CborConverter.Register(typeof(UInt384), new DsonObjectConverter<UInt384>());
            CborConverter.Register(typeof(AID), new DsonObjectConverter<AID>(x => x.Bytes, y => new AID(y)));
        }

        // TODO either add NewInstance(), or make methods static

        public byte[] ToDson<T>(T obj) => ToDsonAsync(obj).Result;

        private async Task<byte[]> ToDsonAsync<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(obj, ms);
                return ms.ToArray();
            }
        }

        public T FromDson<T>(byte[] bytes) => FromDsonAsync<T>(bytes).Result;

        private async Task<T> FromDsonAsync<T>(byte[] bytes)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length); // TODO modify this once .net standard 2.1 is used
                return await Cbor.DeserializeAsync<T>(ms);
            }
        }
    }
}