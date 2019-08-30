using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HeliumParty.RadixDLT.App.Tests.Resources
{
    internal class ResourceParser
    {
        public static string GetResource(string name)
        {
            string result = null;
            using (Stream stream = Assembly.GetExecutingAssembly().
                GetManifestResourceStream($"HeliumParty.RadixDLT.App.Tests.Resources.{name}"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
    }
}
