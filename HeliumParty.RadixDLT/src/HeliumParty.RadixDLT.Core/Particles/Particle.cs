using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles
{
    public abstract class Particle : SerializableObject
    {
        //private readonly HashSet<EUID> _destinations; 
        public HashSet<EUID> Destinations { get; protected set; }

        protected Particle() { }

        public Particle(EUID destination)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            Destinations = new HashSet<EUID>() { destination };
        }

        public Particle(HashSet<EUID> destinations)
        {
            Destinations = destinations ?? throw new ArgumentNullException(nameof(destinations));
        }
    }
}