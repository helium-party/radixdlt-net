using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HeliumParty.DependencyInjection
{
    public class ServiceCollectionRegistrar
    {
        public virtual void AddAssembly(IServiceCollection services, Assembly assembly)
        {
            var types = AssemblyHelper
                .GetAllTypes(assembly)
                .Where(
                    type => (
                                type != null &&
                                type.IsClass &&
                                !type.IsAbstract &&
                                !type.IsGenericType
                            )
                            &&
                            (
                                type.GetInterfaces().Contains(typeof(ITransientDependency)) ||
                                type.GetInterfaces().Contains(typeof(ISingletonDependency)) ||
                                type.GetInterfaces().Contains(typeof(IScopedDependency))
                            )
                ).ToArray();

            AddTypes(services, types);
        }

        public virtual void AddTypes(IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                AddType(services, type);
            }
        }

        public virtual void AddType(IServiceCollection services, Type type)
        {            
            var lifeTime = GetServiceLifetimeFromClassHierarcy(type);

            if (lifeTime == null)
            {
                return;
            }

            var dependencyAttribute = type.GetCustomAttribute<DependencyAttribute>(true);

            var serviceTypes = GetExposedServices(type);

            foreach (var serviceType in serviceTypes)
            {
                var serviceDescriptor = ServiceDescriptor.Describe(serviceType, type, lifeTime.Value);

                if (dependencyAttribute?.ReplaceServices == true)
                {
                    services.Replace(serviceDescriptor);
                }
                else if (dependencyAttribute?.TryRegister == true)
                {
                    services.TryAdd(serviceDescriptor);
                }
                else
                {
                    services.Add(serviceDescriptor);
                }
            }
        }

        protected virtual ServiceLifetime? GetServiceLifetimeFromClassHierarcy(Type type)
        {
            if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Transient;
            }

            if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Singleton;
            }

            if (typeof(IScopedDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Scoped;
            }

            return null;
        }

        private static readonly ExposeServicesAttribute DefaultExposeServicesAttribute =
            new ExposeServicesAttribute
            {
                IncludeDefaults = true
            };

        public static List<Type> GetExposedServices(Type type)
        {
            return type
                .GetCustomAttributes()
                .OfType<IExposedServiceTypesProvider>()
                .DefaultIfEmpty(DefaultExposeServicesAttribute)
                .SelectMany(p => p.GetExposedServiceTypes(type))
                .ToList();
        }

    }
}
