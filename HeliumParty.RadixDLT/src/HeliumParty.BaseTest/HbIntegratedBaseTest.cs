using HeliumParty.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text;

namespace HeliumParty.BaseTest
{
    public abstract class HbIntegratedBaseTest
    {
        public IServiceProvider IocContainer { get; set; }

        public HbIntegratedBaseTest()
        {
            var collection = new ServiceCollection();
            new ServiceCollectionRegistrar()
                .AddAssemblies(collection, AppDomain.CurrentDomain.GetAssemblies());
            IocContainer = collection.BuildServiceProvider();
        }
    }
}
