using HeliumParty.RadixDLT.Particles;

namespace HeliumParty.RadixDLT.DataTransferObjects
{
    public class SpunParticleDTO<T> where T : ParticleDTO
    {
        public Spin Spin { get; set; }

        public T Particle { get; set; }
    }
}