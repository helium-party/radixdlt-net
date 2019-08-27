using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Particle
    {
        [CborProperty("destinations"), JsonProperty(PropertyName = "destinations")]
        private readonly HashSet<EUID> _destinations; // TODO make immutable?

        public Particle(EUID destination)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            _destinations = new HashSet<EUID>() { destination };
        }

        [CborConstructor, JsonConstructor]
        public Particle(HashSet<EUID> destinations)
        {
            _destinations = destinations ?? throw new ArgumentNullException(nameof(destinations));
        }
    }
}