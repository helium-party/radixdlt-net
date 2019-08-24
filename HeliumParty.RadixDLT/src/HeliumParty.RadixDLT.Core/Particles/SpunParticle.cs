namespace HeliumParty.RadixDLT.Particles
{
    public class SpunParticle<T> where T : Particle
    {
        public Spin Spin { get; }
        public T Particle { get; }

        public SpunParticle(T particle, Spin spin)
        {
            Particle = particle;
            Spin = spin;
        }
    }
}