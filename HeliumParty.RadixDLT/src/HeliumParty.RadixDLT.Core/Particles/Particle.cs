using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles
{
    public abstract class Particle
    {
        private readonly HashSet<EUID> _destinations; // TODO make immutable?

        public Particle(EUID destination)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            _destinations = new HashSet<EUID>() { destination };
        }

        public Particle(HashSet<EUID> destinations)
        {
            _destinations = destinations ?? throw new ArgumentNullException(nameof(destinations));
        }
    }
}