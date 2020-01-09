using System.Collections.Generic;
using System.Collections.Immutable;
using Dahomey.Cbor;
using Dahomey.Cbor.Attributes;
using Dahomey.Cbor.ObjectModel;
using HeliumParty.RadixDLT.Core.Tests.Resources;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Particles.Types;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization.Dson;
using Shouldly;
using System.IO;
using System.Threading.Tasks;
using Dahomey.Cbor.Serialization.Conventions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Serialization;
using Xunit;

namespace HeliumParty.RadixDLT.Core.Tests.Serialization.Dson
{
    public class DahomeyTests
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
            options.Registry.DiscriminatorConventionRegistry.RegisterConvention(new AttributeBasedDiscriminatorConvention<string>(options.Registry, "serializer"));
            options.Registry.DiscriminatorConventionRegistry.RegisterType<Dog>();

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
            var cborValue = Bytes.ToHexString(serializedBytes);

            Animal animal = null;
            using (var ms = new MemoryStream())
            {
                ms.Write(serializedBytes, 0, serializedBytes.Length);
                animal = await Cbor.DeserializeAsync<Animal>(ms, options);
            }

            animal.ShouldBeOfType<Dog>();
        }

        [CborDiscriminator("somediscriminator", Policy = CborDiscriminatorPolicy.Always)]
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
            options.Registry.DiscriminatorConventionRegistry.RegisterConvention(new AttributeBasedDiscriminatorConvention<string>(options.Registry));
            options.Registry.DiscriminatorConventionRegistry.RegisterType<Car>();

            byte[] serializedBytes = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(car, ms, options);
                serializedBytes = ms.ToArray();
            }
            
            CborObject cbor = null;
            using (var ms = new MemoryStream())
            {
                ms.Write(serializedBytes, 0, serializedBytes.Length);
                cbor = await Cbor.DeserializeAsync<CborObject>(ms, options);
            }

            cbor.ShouldNotBeNull();
        }

        public abstract class DummyParticle
        {
            protected DummyParticle() { }
        }

        [CborDiscriminator("radix.particles.message", Policy = CborDiscriminatorPolicy.Always)]
        public class DummyMessageParticle : DummyParticle
        {
            public long nonce { get; protected set; }

            public DummyMessageParticle() : base() { }
        }

        [Fact]
        public async Task ShouldDeserialize_Foreign_Cbor_ToParticle_Test()
        {
            CborOptions options = new CborOptions();
            var discriminator = new DsonDiscriminator(options.Registry);
            options.Registry.DiscriminatorConventionRegistry.RegisterConvention(discriminator);
            options.Registry.DiscriminatorConventionRegistry.RegisterType<DummyMessageParticle>();

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


            var manager = new DsonManager();
            var particle2 = manager.FromDson<Particle>(data);
            (particle2 as MessageParticle).Nonce.ShouldBe(2181035975144481159);
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

        [CborDiscriminator("dahomey.immutabletest", Policy = CborDiscriminatorPolicy.Always)]
        public class ImmutableTest
        {
            [SerializationOutput(OutputMode.All)]
            public ImmutableList<Atom> MyList { get; set; }

            public ImmutableTest(List<Atom> list)
            {
                MyList = list?.ToImmutableList();
            }
        }

        [Fact]
        public void Should_Serialize_And_Deserialize()
        {
            var manager = new DsonManager();

            var obj = new ImmutableTest(new List<Atom>{new Atom()});

            var serialized = manager.ToDson(obj, OutputMode.Hash);
            var deserialized = manager.FromDson<ImmutableTest>(serialized, OutputMode.Hash);
        }
    }
}
