using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Reflection;
using Xunit;

namespace HeliumParty.DependencyInjection.Tests
{
    public class BasicDI_Tests
    {
        private const string SUCCESS = "DI succes";
        public interface ISomeService
        {
            string ReturnSomething();
        }

        public class SomeService : ISomeService, ITransientDependency
        {
            public string ReturnSomething()
            {
                return SUCCESS;
            }
        }

        public class SomeClass
        {
            private readonly ISomeService _service;

            public SomeClass(ISomeService service)
            {
                _service = service;
            }

            public string Act()
            {
                return _service.ReturnSomething();
            }
        }

        [Fact]
        public void BasicDiTest()
        {
            //arrange
            var collection = new ServiceCollection();
            new ServiceCollectionRegistrar().AddAssembly(collection, Assembly.GetExecutingAssembly());
            collection.AddTransient<SomeClass>();
            var serviceProvider = collection.BuildServiceProvider();

            //act
            var c = serviceProvider.GetService<SomeClass>();
            var r = c.Act();
            //assert
            r.ShouldBe(SUCCESS);
        }
    }
}
