using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles;
using System;
using System.Collections.Generic;

namespace HeliumParty.RadixDLT.Ledger
{
    /// <summary>
    /// The interface in which a client retrieves the state of the ledger.
    /// Particle conflic handling along iwth Atom DELETEs and STOREs all 
    /// occur at this layer.
    /// </summary>
    public interface IAtomStore
    {
        /// <summary>
        /// Temporary interface for propagating when the current store is 
        /// synced with some node on a given address.
        /// 
        /// // TODO: Might get refactored in java later.
        /// 
        /// </summary>
        /// <param name="address">The address to check for sync</param>
        /// <returns>A never ending observable which emits when local store 
        /// is synced with some origin</returns>
        IObservable<long> OnSync(RadixAddress address);

        /// <summary>
        /// Retrieve the current set of validated atoms at a given shardable
        /// </summary>
        /// <param name="address">The address to get the atoms under</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all stored atoms of the current local view</returns>
        IEnumerable<Atom> GetStoredAtoms(RadixAddress address);

        /// <summary>
        /// Retrieve a never ending observable of atom observations (STORED and DELETED)
        /// which are then processed by the local store
        /// </summary>
        /// <param name="address">The address to get the updates from</param>
        /// <returns>A never ending observable of updates</returns>
        IObservable<AtomObservation> GetAtomObservations(RadixAddress address);

        /// <summary>
        /// Retrieve the current set of validated up particles at a given shardable
        /// If <paramref name="uuid"/> is provided, staged particles under that 
        /// <paramref name="uuid"/> are also retrieved.
        /// </summary>
        /// <param name="address">The address to get the particles under</param>
        /// <param name="uuid"><paramref name="uuid"/> of staged particles to include</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all up particles of the current local view</returns>
        IEnumerable<Particle> GetUpParticles(RadixAddress address, string uuid = null);

        /// <summary>
        /// Adds the particle group to the staging area for the given <paramref name="uuid"/>
        /// </summary>
        /// <param name="uuid">The <paramref name="uuid"/> to add the particle group to</param>
        /// <param name="particleGroup">The <see cref="ParticleGroup"/> to add to the staging area</param>
        void StageParticleGroup(string uuid, ParticleGroup particleGroup);

        /// <summary>
        /// Retrieves all staged particle groups and clears the staging area 
        /// for the given <paramref name="uuid"/>
        /// 
        /// // TODO: Might get refactored in java later.
        /// 
        /// </summary>
        /// <param name="uuid"><paramref name="uuid"/> to retrieve the staged particle groups for</param>
        /// <returns>All staged <see cref="ParticleGroup"/>s in the order they were staged</returns>
        List<ParticleGroup> GetStagedAndClear(string uuid);
    }
}
