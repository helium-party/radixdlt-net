using HeliumParty.RadixDLT.Core.Tests.Resources;
using HeliumParty.RadixDLT.Pow;
using HeliumParty.RadixDLT.Serialization.Dson;
using Shouldly;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using HeliumParty.BaseTest;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Hashing;

namespace HeliumParty.RadixDLT.Core.Tests.Pow
{
    public class ProofOfWork_Tests : HbIntegratedBaseTest
    {
        private readonly byte[] _atomDson;
        //private readonly IDsonManager _dsonManager;

        public ProofOfWork_Tests()
        {
            _atomDson = ResourceParser.GetResource("messageatom.dson");
            //_dsonManager = IocContainer.GetService<IDsonManager>();
        }

        [Fact]
        public void ShouldGenerate_Valid_Pow()
        {
            //arrange
            var powB = new ProofOfWorkBuilder();
            var seed = RadixHash.From(_atomDson).ToByteArray();

            //act
            var pow = powB.Build(266731521, seed, 16);

            //assert
            pow.ShouldNotBeNull();            
            pow.Validate();
        }
    }
}
