using System.IO;
using System.Threading.Tasks;
using Dahomey.Cbor;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonManager
    {
        public byte[] ToDson<T>(T obj, OutputMode mode = OutputMode.All) => ToDsonAsync(obj, mode).Result;        

        public T FromDson<T>(byte[] bytes, OutputMode mode = OutputMode.All) => FromDsonAsync<T>(bytes, mode).Result;

        public async Task<byte[]> ToDsonAsync<T>(T obj, OutputMode mode)
        {
            using (var ms = new MemoryStream())
            {
                //var t = DsonOutputMapping.GetDsonOptions();
                var options = DsonOutputMapping.GetDsonOptions(mode);
                await Cbor.SerializeAsync(obj, ms, options);
                return ms.ToArray();                
            }
        }

       

        public async Task<T> FromDsonAsync<T>(byte[] bytes, OutputMode mode = OutputMode.All)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length); // TODO modify this once .net standard 2.1 is used
                return await Cbor.DeserializeAsync<T>(ms, DsonOutputMapping.GetDsonOptions(mode));
            }
        }
    }
}
