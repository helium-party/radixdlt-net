using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.State;
using System;


namespace HeliumParty.RadixDLT
{
    public interface IParticleReducer
    {

    }
    public interface IParticleReducer<T> : IParticleReducer
        where T : IApplicationState
    {
        Type StateClass();
        T InitialState();
        T Reduce(T state, Particle p);
        T Combine(T state0, T state1);
    }
}
