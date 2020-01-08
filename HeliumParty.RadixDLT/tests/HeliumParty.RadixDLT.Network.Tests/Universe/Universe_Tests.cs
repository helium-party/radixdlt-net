using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Serialization.Json;
using HeliumParty.RadixDLT.Universe;
using System.Collections.Generic;
using Xunit;
using Shouldly;
using HeliumParty.BaseTest;
using Microsoft.Extensions.DependencyInjection;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using System.Linq;
using HeliumParty.RadixDLT.EllipticCurve.Managers;

namespace HeliumParty.RadixDLT.Network.Tests
{
    public class Universe_Tests : HbIntegratedBaseTest
    {
        private readonly IJsonManager _jsonmanager;
        private readonly Identity.Managers.IEUIDManager _euidmanager;

        public Universe_Tests()
        {
            _jsonmanager = IocContainer.GetService<IJsonManager>();
            _euidmanager = IocContainer.GetService<Identity.Managers.IEUIDManager>();
        }

        [Fact]
        public void UniverseConfigsLoadTest()
        {
            // Test if we can load our configuration files
            // Note that any occuring exception determines the unit test as failure
            RadixUniverseConfigs.GetAlphanet();
            RadixUniverseConfigs.GetBetanet();
            RadixUniverseConfigs.GetHighgarden();
            RadixUniverseConfigs.GetLocalnet();
            RadixUniverseConfigs.GetSunstone();
            RadixUniverseConfigs.GetWinterfell();
        }

        [Fact]
        public void HashUniverseConfigWithoutError()
        {
            var universeConfig = new RadixUniverseConfig(
                name: "sdf",
                description: "asdf",
                systemPublicKey: new ECKeyManager().GetRandomKeyPair().PublicKey,
                timestamp: System.DateTime.UtcNow.Ticks,
                type: RadixUniverseType.Development,
                port: 1,
                magic: 1,
                genesis: new List<Atom>()
                );

            universeConfig.GetHash();
        }

        [Fact]
        public void UniverseConfigEqualityTest()
        {
            var config = RadixUniverseConfigs.GetAlphanet();
            var config1 = RadixUniverseConfigs.GetAlphanet();
            config1.ShouldBe(config);

            var custom_genesis = new Atom[config.Genesis.Count];
            config.Genesis.CopyTo(custom_genesis);

            var custom_config = new RadixUniverseConfig(
                config.Name,
                config.Description,
                config.SystemPublicKey,
                config.Timestamp,
                config.Type,
                config.Port,
                (int)config.Magic,
                custom_genesis.ToList());

            custom_config.ShouldBe(config);
        }

        [Fact]
        public void UniverseConfigFrom_Test()
        {
            //var universe = RadixUniverseConfig.FromBytes(Resources.betanet);

            //var expectedUniverse = new RadixUniverseConfig(
            //    name: "Radix Devnet",
            //    description: "The Radix test Universe",
            //    systemPublicKey: new EllipticCurve.ECPublicKey("A3hanCWf3pmR5E+i+wtWWfKleBrDOQduLb/vcFKOSt9o"),
            //    timestamp: 1551225600000,
            //    type: RadixUniverseType.Development,
            //    port: 30000,
            //    magic: -1332248574,
            //    genesis: new List<Atoms.Atom>()
            //    {
            //        new Atoms.Atom(
            //            particleGroups: new List<ParticleGroup>()
            //            {
            //                new ParticleGroup(),
            //                new ParticleGroup(),
            //                new ParticleGroup()
            //            },
            //            timestamp: 1551225600000)
            //    }
            //    );

            //universe.Name.ShouldBe(expectedUniverse.Name);
            //universe.Magic.ShouldBe(expectedUniverse.Magic);
            //universe.Description.ShouldBe(expectedUniverse.Description);

            //universe.Port.ShouldBe(expectedUniverse.Port);
            //universe.SystemPublicKey.ShouldBe(expectedUniverse.SystemPublicKey);
            //universe.Genesis.Count.ShouldBe(expectedUniverse.Genesis.Count);
            //universe.Type.ShouldBe(expectedUniverse.Type);
            //universe.Timestamp.ShouldBe(expectedUniverse.Timestamp);
        }



    }
}
