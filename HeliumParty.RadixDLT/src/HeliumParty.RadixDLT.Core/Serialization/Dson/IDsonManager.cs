using System.Threading.Tasks;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public interface IDsonManager
    {
        T FromDson<T>(byte[] bytes, OutputMode mode = OutputMode.All);
        Task<T> FromDsonAsync<T>(byte[] bytes, OutputMode mode = OutputMode.All);
        byte[] ToDson<T>(T obj, OutputMode mode = OutputMode.All);
        Task<byte[]> ToDsonAsync<T>(T obj, OutputMode mode);
    }
}