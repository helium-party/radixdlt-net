using Microsoft.Extensions.DependencyInjection;
using System;

namespace HeliumParty.Autofac.Test
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            RegisterServices();
            var executer = _serviceProvider.GetService<Executer>();
            executer.Run();
            DisposeServices();
        }


        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            collection.AddTransient<Executer>();
            collection.AddTransient<ISomeService, SomeService>();            
            // ...
            // Add other services
            // ...
            _serviceProvider = collection.BuildServiceProvider();
        }
        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
