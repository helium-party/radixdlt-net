namespace HeliumParty.RadixDLT.Serialization.Json
{
    public interface IJsonManager
    {
        T FromJson<T>(string str, OutputMode mode = OutputMode.All);

        string ToJson<T>(T obj, OutputMode mode = OutputMode.All);
    }
}