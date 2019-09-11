using Dahomey.Cbor;
using Dahomey.Cbor.Attributes;
using Dahomey.Cbor.ObjectModel;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Dson;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        [CborDiscriminator("dog.dog")]
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

        public abstract class SerializableObject
        {
            public string _T { get; set; }
        }

        [CborDiscriminator("Car")]
        public class Car : SerializableObject
        {
            public double Value { get; set; }
        }

        [CborDiscriminator("Volvo")]
        public class Volvo : Car
        {
            public string EngineType { get; set; }

            public Volvo()
            {
                Value = 120;
            }
        }

        [CborDiscriminator("Volvo_V60")]
        public class Volvo_V60 : Volvo
        {
            public string Options { get; set; }

            public Volvo_V60():base()
            {
                EngineType = "ELEC";
            }
        }

        [Fact]
        public async Task Should_Deserialize_List_AccordingToDiscriminator()
        {
            //arrange
            var car = new Car()
            {
                Value = 100
            };
            var volvo = new Volvo()
            {                
                EngineType = "DIESEL_PETROL"
            };
            var v60 = new Volvo_V60()
            {
                Options = "someoptions"
            };

            var carlist = new List<SerializableObject>() { car,
                volvo, v60 };

            CborOptions options = new CborOptions();
            options.Registry.DefaultDiscriminatorConvention.RegisterAssembly(typeof(Car).Assembly);

            byte[] serializedBytes = null;
            using (var ms = new MemoryStream())
            {
                await Cbor.SerializeAsync(carlist, ms, options);
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

        }
    }
}
