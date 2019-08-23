using System;
using System.Collections.Generic;

namespace HeliumParty.RadixDLT.Identity
{
    public class RRI
    {
        public RadixAddress Address { get; }
        public string Name { get; }

        public RRI(RadixAddress address, string name)
        {
            Address = address;
            Name = name;
        }

        public RRI(string s)
        {
            var split = s.Split('/');
            if (split.Length < 2)
                throw new ArgumentException($"{nameof(RRI)} must be of the format /:address/:name");

            var address = new RadixAddress(split[1]);
            var name = s.Substring(split[1].Length + 2); // TODO is this correct?

            Address = address;
            Name = name;
        }

        public override string ToString()
        {
            return $"/{Address.ToString()}/{Name}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RRI))
            {
                return false;
            }

            var rri = (RRI)obj;

            return Address.Equals(rri.Address) && rri.Name == Name;
        }

        public override int GetHashCode()
        {
            var hashCode = 389620771;
            hashCode = hashCode * -1521134295 + EqualityComparer<RadixAddress>.Default.GetHashCode(Address);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}