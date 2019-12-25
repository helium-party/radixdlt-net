using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using HeliumParty.Collections;

namespace HeliumParty.RadixDLT.Ledger
{
    /// <summary>
    /// An in memory storage of atoms and particles
    /// </summary>
    public class InMemoryAtomStore : IAtomStore
    {
        #region Private Members

        private readonly ConcurrentDictionary<Atom, AtomObservation> _Atoms = 
            new ConcurrentDictionary<Atom, AtomObservation>();

        private readonly ConcurrentDictionary<Particle, Dictionary<Spin, HashSet<Atom>>> _ParticleIndex =
            new ConcurrentDictionary<Particle, Dictionary<Spin, HashSet<Atom>>>();

        private readonly ConcurrentDictionary<RadixAddress, List<IObserver<AtomObservation>>> _AllObservers = 
            new ConcurrentDictionary<RadixAddress, List<IObserver<AtomObservation>>>();

        private readonly ConcurrentDictionary<RadixAddress, List<IObserver<long>>> _AllSyncers =
            new ConcurrentDictionary<RadixAddress, List<IObserver<long>>>();

        private readonly object _Lock = new object();
        private readonly Dictionary<RadixAddress, bool> _SyncedMap;

        private readonly ConcurrentDictionary<string, Atom> _StagedAtoms = new ConcurrentDictionary<string, Atom>();

        private readonly ConcurrentDictionary<string, Dictionary<Particle, Spin>> _StagedParticleIndex = 
            new ConcurrentDictionary<string, Dictionary<Particle, Spin>>();

        #endregion

        #region Interface Methods

        public IObservable<AtomObservation> GetAtomObservations(RadixAddress address)
        {
            return Observable.Create<AtomObservation>(emitter =>
            {
                lock (_Lock)
                {
                    var observers = _AllObservers.GetOrAdd(address, new List<IObserver<AtomObservation>>());
                    observers.Add(emitter);
                    
                    var observations = _Atoms.Where(e => e.Value.IsStore && e.Key.GetAllAddresses().Any(addr => addr.Equals(address)))
                        .Select(e => e.Value);

                    foreach (var observation in observations)
                        emitter.OnNext(observation);

                    return Disposable.Create(() =>
                    {
                        lock (_Lock)
                        {
                            observers.Remove(emitter);
                        }
                    });
                }
            });
        }

        public List<ParticleGroup> GetStagedAndClear(string uuid)
        {
            if (uuid == null)
                throw new ArgumentNullException(nameof(uuid));

            lock (_Lock)
            {
                _StagedAtoms.TryRemove(uuid, out var atom);
                _StagedParticleIndex.TryGetValue(uuid, out var dic);
                dic?.Clear();
                return atom?.ParticleGroups;
            }
        }

        public IEnumerable<Atom> GetStoredAtoms(RadixAddress address)
        {
            lock (_Lock)
            {
                return _Atoms
                    .Where(e => e.Value.IsStore && e.Key.GetAllAddresses().Any(addr => addr.Equals(address)))
                    .Select(e => e.Value)
                    .Select(obs => obs.ObservedAtom);
            }
        }

        public IEnumerable<Particle> GetUpParticles(RadixAddress address, string uuid = null)
        {
            lock (_Lock)
            {
                var upParticles = _ParticleIndex.Where(e =>
                {
                    if (!e.Key.GetShareables().Contains(address))
                        return false;

                    var spinParticleIndex = e.Value;
                    if (spinParticleIndex.TryGetValue(Spin.Down, out var spinAtoms))
                        if (DetermineAnyIsStore(spinAtoms))
                            return true;

                    if (uuid != null)
                        if (_StagedParticleIndex.TryGetValue(uuid, out var dic))
                            if (dic.TryGetValue(e.Key, out var spin))
                                if (spin == Spin.Down)
                                    return false;

                    if (spinParticleIndex.TryGetValue(Spin.Up, out spinAtoms))
                        return DetermineAnyIsStore(spinAtoms);

                    return false;
                })
                .Select(e => e.Key)
                .ToList();

                if (uuid != null)
                    if (_StagedParticleIndex.TryGetValue(uuid, out var dic))
                        upParticles.AddRange(dic.Where(e => e.Value == Spin.Up).Select(e => e.Key));

                return upParticles;
            }
        }

        public IObservable<long> OnSync(RadixAddress address)
        {
            return Observable.Create<long>(emitter =>
            {
                lock (_Lock)
                {
                    if (_SyncedMap.TryGetValue(address, out bool synced) && synced)
                        emitter.OnNext(System.DateTime.Now.Ticks);

                    var syncers = _AllSyncers.GetOrAdd(address, new List<IObserver<long>>());
                    syncers.Add(emitter);

                    return Disposable.Create(() =>
                    {
                        lock (_Lock)
                        {
                            syncers.Remove(emitter);
                        }
                    });
                }
            });
        }

        public void StageParticleGroup(string uuid, ParticleGroup particleGroup)
        {
            if (uuid == null)
                throw new ArgumentNullException(nameof(uuid));
            if (particleGroup == null)
                throw new ArgumentNullException(nameof(particleGroup));

            lock (_Lock)
            {
                _StagedAtoms.TryGetValue(uuid, out var stagedAtom);
                if (stagedAtom == null)
                    stagedAtom = new Atom(particleGroup, System.DateTime.UtcNow.Ticks);
                else
                {
                    var groups = stagedAtom.ParticleGroups.Concat(new List<ParticleGroup>{ particleGroup }).ToList();
                    stagedAtom = new Atom(groups, System.DateTime.UtcNow.Ticks);
                }

                _StagedAtoms.AddOrUpdate(uuid, stagedAtom, (_1, _2) => stagedAtom);

                foreach (var sp in particleGroup.Particles)
                {
                    _StagedParticleIndex.TryGetValue(uuid, out var spinByParticle);
                    if (spinByParticle == null)
                        spinByParticle = new Dictionary<Particle, Spin>();

                    spinByParticle.AddOrUpdate(sp.Particle, sp.Spin);
                    _StagedParticleIndex.AddOrUpdate(uuid, spinByParticle, (_1, _2) => spinByParticle);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Store an atom under a given destination
        /// </summary>
        /// <param name="address">Address to store under</param>
        /// <param name="observation">The atom to store</param>
        public void Store(RadixAddress address, AtomObservation observation)
        {
            lock (_Lock)
            {
                var synced = observation.IsHead;
                _SyncedMap.AddOrUpdate(address, synced);

                var atom = observation.ObservedAtom;
                if (atom != null)
                {
                    _Atoms.TryGetValue(atom, out var currentObservation);
                    var nextUpdate = observation.UpdateState;
                    var lastUpdate = currentObservation?.UpdateState;

                    // If a new hard observed atom conflicts with a previously stored atom, 
                    // stored atom must be deleted.
                    if (nextUpdate.StorageType == AtomStorageType.Store && !nextUpdate.IsSoft)
                    {
                        foreach (var sp in atom.GetAllParticles())
                        {
                            if (!_ParticleIndex.TryGetValue(sp.Particle, out var atomsBySpin))
                                continue;

                            if (!atomsBySpin.TryGetValue(sp.Spin, out var atoms))
                                continue;

                            foreach (var a in atoms)
                            {
                                if (a.Equals(atom))
                                    continue;

                                _Atoms.TryGetValue(a, out var oldObservation);
                                if (oldObservation != null && oldObservation.IsStore)
                                {
                                    SoftDeleteDependentsOf(a);
                                    _Atoms.AddOrUpdate(a, AtomObservation.SoftDeleted(a));
                                }
                            }
                        }
                    }

                    bool include;
                    if (lastUpdate == null)
                    {
                        include = nextUpdate.StorageType == AtomStorageType.Store;
                        foreach (var sp in atom.GetAllParticles())
                        {
                            _ParticleIndex.TryGetValue(sp.Particle, out var atomsBySpin);
                            if (atomsBySpin == null)
                            {
                                atomsBySpin = new Dictionary<Spin, HashSet<Atom>>();
                                _ParticleIndex.AddOrUpdate(sp.Particle, atomsBySpin, (_1, _2) => atomsBySpin);
                            }

                            atomsBySpin.MergeValues(
                                sp.Spin,
                                new HashSet<Atom>() { atom },
                                (oldVal, newVal) => new HashSet<Atom>(oldVal.Concat(newVal)));
                        }
                    }
                    else
                    {
                        // Soft observation should not be able to update a hard state
                        // Only update if type changes
                        include = (!nextUpdate.IsSoft || lastUpdate.IsSoft) 
                            && nextUpdate.StorageType != lastUpdate.StorageType;
                    }

                    if (nextUpdate.StorageType == AtomStorageType.Delete && include)
                        SoftDeleteDependentsOf(atom);

                    var isSoftToHard = lastUpdate != null && lastUpdate.IsSoft && !nextUpdate.IsSoft;
                    if (include || isSoftToHard)
                        _Atoms.AddOrUpdate(atom, observation, (_1, _2) => observation);

                    // Notify all observers about new observation
                    if (include)
                        foreach (var addr in atom.GetAllAddresses())
                            if (_AllObservers.TryGetValue(addr, out var observers))
                                foreach (var obs in observers)
                                    obs.OnNext(observation);
                }

                else
                {
                    // Notify all observers about new observation
                    if (_AllObservers.TryGetValue(address, out var observers))
                        foreach (var obs in observers)
                            obs.OnNext(observation);
                }

                // Notify every syncer of current system time
                if (synced)
                    if (_AllSyncers.TryGetValue(address, out var syncers))
                        foreach (var sync in syncers)
                            sync.OnNext(System.DateTime.UtcNow.Ticks);
            }
        }

        #endregion

        #region Private Methods

        private bool DetermineAnyIsStore(IEnumerable<Atom> atoms)
        {
            return atoms.Any(a =>
            {
                _Atoms.TryGetValue(a, out var observation);
                return observation.IsStore;
            });
        }

        private void SoftDeleteDependentsOf(Atom atom)
        {
            foreach (var p in atom.GetAllParticles(Spin.Up))
            {
                if (!_ParticleIndex.TryGetValue(p, out var atomsBySpin))
                {
                    if (atomsBySpin.TryGetValue(Spin.Down, out var atoms))
                    {
                        foreach (var a in atoms)
                        {
                            if (!_Atoms.TryGetValue(a, out AtomObservation observation))
                                break;

                            var observedAtom = observation.ObservedAtom;
                            if (observedAtom.Equals(a))
                                break;

                            if (observation.UpdateState.StorageType == AtomStorageType.Store ||
                                !observation.UpdateState.IsSoft)
                            {
                                SoftDeleteDependentsOf(observedAtom);
                                var newObservation = AtomObservation.SoftDeleted(observedAtom);
                                _Atoms.AddOrUpdate(observedAtom, newObservation, (_1, _2) => newObservation);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
