//using HeliumParty.RadixDLT.Identity;
//using HeliumParty.RadixDLT.Primitives;

//namespace HeliumParty.RadixDLT.Particles.Types
//{
//    public abstract class TokenDefinitionParticle : Particle, IIdentifiable, IOwnable
//    {
//        public RRI RRI { get; protected set; }
//        public RadixAddress Address => RRI.Address;
//        public string Name { get; protected set; }
//        public string Description { get; protected set; }
//        public UInt256 Granularity { get; protected set; }
//        public string IconUrl { get; protected set; }

//        protected TokenDefinitionParticle () : base () { }

//        protected TokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl)
//            : base(rRI.Address.EUID)
//        {
//            RRI = rRI;
//            Name = name;
//            Description = description;
//            Granularity = granularity;
//            IconUrl = iconUrl;
//        }
//    }
//}