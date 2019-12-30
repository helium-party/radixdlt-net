using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Serialization.Json;
using HeliumParty.RadixDLT.Universe;
using System.Collections.Generic;
using Xunit;
using Shouldly;
using HeliumParty.BaseTest;
using Microsoft.Extensions.DependencyInjection;

namespace HeliumParty.RadixDLT.Network.Tests
{
    public class Universe_Tests : HbIntegratedBaseTest
    {
        private readonly IJsonManager _jsonmanager;

        public Universe_Tests()
        {
            _jsonmanager = IocContainer.GetService<IJsonManager>();
        }

        [Fact]
        public void UniverseConfigFrom_Test()
        {
            //var universe = RadixUniverseConfig.FromBytes(Resources.betanet);

            //var expectedUniverse = new RadixUniverseConfig(
            //    name: "radix.universe",
            //    description: "The Radix test Universe",
            //    systemPublicKey: new EllipticCurve.ECPublicKey("Agzfn1Z35c749BQlEGUonphiGt2BIntgL/Zq9py9rk7h"),
            //    timestamp: 1551225600000,
            //    type: RadixUniverseType.Development,
            //    port: 20000,
            //    magic: 266731521,
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
