using System.IO;
using System.Threading.Tasks;
using Dahomey.Cbor;
using HeliumParty.RadixDLT.Mapping;

namespace HeliumParty.RadixDLT
{
    public class DsonManager
    {
        public byte[] ToDson<T>(T obj, OutputMode mode = OutputMode.All)
        {
            return ToDsonAsync(obj, mode).Result;
        }

        private async Task<byte[]> ToDsonAsync<T>(T obj, OutputMode mode)
        {
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(obj, ms, DsonOutputMapping.GetCborOptions(mode));
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