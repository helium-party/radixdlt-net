using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Mapping;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT
{
    public class JsonManager
    {
        public string ToJson(object o, OutputMode mode = OutputMode.All) => JsonConvert.SerializeObject(o, JsonOutputMapping.GetJsonOptions(mode));

        public T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json, JsonOutputMapping.GetJsonOptions(OutputMode.All));
    }
}