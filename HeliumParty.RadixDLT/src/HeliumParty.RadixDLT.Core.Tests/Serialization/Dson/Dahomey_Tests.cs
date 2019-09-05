using Dahomey.Cbor;
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

        //class TestClass
        //{
        //    [SerializationOutput(OutputMode.Hash | OutputMode.Persist | OutputMode.All)]
        //    public string VisibleField { get; set; }
        //    [SerializationOutput(OutputMode.None)]
        //    public string HiddenField { get; set; }
        //}

        class Person
        {
            //do not serialize Id field on insert/creation of object
            public string Id { get; set; }
            public string SomeField { get; set; }
        }

        class PersonInsert
        {
            public string SomeField { get; set; }
        }

        class PersonDelete
        {
            public string Id { get; set; }
        }

        //[Fact]
        //public async Task Should_Not_Serialize_Forbidden_Fields()
        //{
        //    var options = CborOptions.Default;
        //    options.Registry.ConverterRegistry.RegisterConverter(typeof(TestClass),
        //        new DsonObjectConverter<TestClass>(OutputMode.Persist);

        //    CborOptions.Default.Registry.ObjectMappingRegistry.Register<TestClass>(om =>
        //    {
        //        om.AutoMap();
        //        om.ClearMemberMappings();
        //        om.MapMember(o => o.VisibleField);

        //    });
        //}
    }
}
