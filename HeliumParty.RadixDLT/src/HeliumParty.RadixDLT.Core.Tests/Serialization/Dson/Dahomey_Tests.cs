using Dahomey.Cbor;
using Dahomey.Cbor.Attributes;
using Dahomey.Cbor.ObjectModel;
using HeliumParty.RadixDLT.Core.Tests.Resources;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization.Dson;
using Shouldly;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace HeliumParty.RadixDLT.Core.Tests.Serialization.Dson
{
    public class Dahomey_Tests
    {

        [Fact]
        public async Task Should_Deserialize_WithCustom_Options()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var options = new CborOptions();
            options.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]),
                new DsonObjectConverter<byte[]>(x => x, y => y));

            byte[] serializedBytes = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(bytes, ms, options);
                serializedBytes = ms.ToArray();
            }

            byte[] deserializedBytes = null;
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(serializedBytes, 0, serializedBytes.Length); 
                deserializedBytes = await Cbor.DeserializeAsync<byte[]>(ms, options);
            }

            deserializedBytes.ShouldBe(bytes);
        }

        [Fact]
        public async Task Should_Deserialize_WithDefault_Options()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var options = CborOptions.Default;
            options.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]),
                new DsonObjectConverter<byte[]>(x => x, y => y));

            byte[] serializedBytes = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(bytes, ms, options);
                serializedBytes = ms.ToArray();
            }

            byte[] deserializedBytes = null;
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(serializedBytes, 0, serializedBytes.Length); 
                deserializedBytes = await Cbor.DeserializeAsync<byte[]>(ms, CborOptions.Default);
            }

            deserializedBytes.ShouldBe(bytes);
        }

        public abstract class Animal
        {
            public int Sound { get; set; }
        }

        [CborDiscriminator("dog.dog", Policy = CborDiscriminatorPolicy.Always)]
        public class Dog : Animal
        {
            public int OtherSound { get; set; }
        }

        [Fact]
        public async Task Should_Deserialize_AccordingToDiscriminator()
        {
            CborOptions options = new CborOptions();
            options.Registry.DefaultDiscriminatorConvention.RegisterAssembly(typeof(Dog).Assembly);

            var dog = new Dog()
            {
                Sound = 5,
                OtherSound = 11
            };

            byte[] serializedBytes = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(dog, ms, options);
                serializedBytes = ms.ToArray();
            }

            Animal animal = null;
            using (var ms = new MemoryStream())
            {
                ms.Write(serializedBytes, 0, serializedBytes.Length);
                animal = await Cbor.DeserializeAsync<Animal>(ms, options);
            }

            animal.ShouldBeOfType<Dog>();


        }



        [CborDiscriminator("somediscriminator")]
        public class Car 
        {
            public string Description{ get; set; }
        }


        
        [Fact]
        public async Task Should_Serialize_Discriminator()
        {
            //arrange
            var car = new Car()
            {
                Description = "n"
            };
            

            CborOptions options = new CborOptions();
            options.Registry.DefaultDiscriminatorConvention.RegisterAssembly(typeof(Car).Assembly);

            byte[] serializedBytes = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(car, ms, options);
                serializedBytes = ms.ToArray();
            }

            //List<Car> cbor = null;
            CborObject cbor = null;
            using (var ms = new MemoryStream())
            {
                ms.Write(serializedBytes, 0, serializedBytes.Length);
                cbor = await Cbor.DeserializeAsync<CborObject>(ms, options);
            }

            cbor.ShouldNotBeNull();

            var actualoutput = Bytes.ToHexString(serializedBytes);
            cbor.Add("_t", "somediscriminator");

            byte[] serializedBytes2 = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(cbor, ms, options);
                serializedBytes2 = ms.ToArray();
            }

            var expectedOutput = Bytes.ToHexString(serializedBytes2);
            expectedOutput.ShouldNotBeNull();
        }


        public abstract class DummyParticle
        {

        }

        [CborDiscriminator("radix.particles.message", Policy = CborDiscriminatorPolicy.Always)]
        public class DummyMessageParticle : DummyParticle
        {
            public long nonce { get; protected set; }
        }

        [Fact]
        public async Task ShouldDeserialize_Foreign_Cbor_ToParticle_Test()
        {
            //CborOptions options = DsonOutputMapping.GetDsonOptions(OutputMode.All);
            CborOptions options = new CborOptions();
            var discriminator = new DsonDiscriminator();
            discriminator.RegisterAssembly(Assembly.GetAssembly(typeof(DummyParticle)));
            options.DiscriminatorConvention = discriminator;




            var data = ResourceParser.GetResource("messageParticle3.dson");
            DummyParticle particle = null;
            using (var ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                particle = await Cbor.DeserializeAsync<DummyParticle>(ms, options);//fails
                //particle = await Cbor.DeserializeAsync<DummyMessageParticle>(ms, options);//works               
            }

            particle.ShouldBeOfType<DummyMessageParticle>();
            ((DummyMessageParticle)particle).nonce.ShouldBe(2181035975144481159);
        }


        public class ConstTest
        {
            public int Value { get; set; }

            protected ConstTest()
            {

            }

            public ConstTest(int value)
            {
                Value = value;
            }
        }

        [Fact]
        public async Task ProtectedConstructorTest()
        {
            var test = new ConstTest(100);
            CborOptions options = new CborOptions();


            byte[] serializedBytes = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(test, ms, options);
                serializedBytes = ms.ToArray();
            }

            ConstTest deserialized = null;
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(serializedBytes, 0, serializedBytes.Length);
                deserialized = await Cbor.DeserializeAsync<ConstTest>(ms, options);
            }

            deserialized.Value.ShouldBe(100);

        }
    }
}
