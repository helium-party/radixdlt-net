using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Mapping;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT
{
    public class JsonManager
    {
        static JsonManager()
        {
            // registering custom json converters
            var jsonConverters = new JsonConverterCollection
            {
                new JsonObjectConverter<byte[]>(Bytes.ToBase64String, Bytes.FromBase64String),
                new JsonObjectConverter<EUID>(x => x.ToString(), y => new EUID(y)),
                //new JsonObjectConverter<Hash>(x => x.ToString(), y => new Hash(y)),
                new JsonObjectConverter<string>(x => x, y => y),
                new JsonObjectConverter<RadixAddress>(x => x.ToString(), y => new RadixAddress(y)),
                //new JsonObjectConverter<UInt256>(x => x.ToString(), UInt256.From),
                new JsonObjectConverter<RRI>(x => x.ToString(), y => new RRI(y)),
                //new JsonObjectConverter<UInt384>(),
                new JsonObjectConverter<AID>(x => x.ToString(), y => new AID(y)),
            };
        }
    }
}