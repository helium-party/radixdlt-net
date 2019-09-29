using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;


namespace HeliumParty.DependencyInjection
{
    internal static class AssemblyHelper
    {
        public static List<Assembly> LoadAssemblies(string folderPath, SearchOption searchOption)
        {
            return GetAssemblyFiles(folderPath, searchOption)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                .ToList();
        }

        public static IEnumerable<string> GetAssemblyFiles(string folderPath, SearchOption searchOption)
        {
            return Directory
                .EnumerateFiles(folderPath, "*.*", searchOption)
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));
        }

        public static IReadOnlyList<Type> GetAllTypes(Assembly assembly, bool includeDependentAssemblies = false)
        {
            //var assemblies = assembly.GetReferencedAssemblies().ToList();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = new List<Type>();
            
            foreach(var a in assemblies)
            {
                try
                {
                    types.AddRange(a.GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types.AddRange(ex.Types);
                }
            }

            return types;
        }

        public static IReadOnlyList<Type> GetAllTypes(Assembly assembly)
        {
            //var assemblies = assembly.GetReferencedAssemblies().ToList();

            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //var types = new List<Type>();

           
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }            
        }

        public static IReadOnlyList<Type> GetAllTypes(Assembly[] assemblies)
        {
            var types = new List<Type>();

            foreach (var a in assemblies)
            {
                types.AddRange(GetAllTypes(a));
            }

            return types;
        }
    }
}
