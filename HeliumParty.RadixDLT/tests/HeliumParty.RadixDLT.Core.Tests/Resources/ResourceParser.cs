using System.IO;
using System.Reflection;

namespace HeliumParty.RadixDLT.Core.Tests.Resources
{
    internal class ResourceParser
    {
        public static byte[] GetResource(string name)
        {
            byte[] result = null;
            using (MemoryStream ms = new MemoryStream())
            {
                var st = Assembly.GetExecutingAssembly().
                GetManifestResourceStream($"HeliumParty.RadixDLT.Core.Tests.Resources.{name}");

                st.CopyTo(ms);
                
                //using (StreamReader reader = new StreamReader(stream))
                //{
                //    result = reader.b
                //}
                result = ms.ToArray();
            }
            return result;
        }
    }
}
