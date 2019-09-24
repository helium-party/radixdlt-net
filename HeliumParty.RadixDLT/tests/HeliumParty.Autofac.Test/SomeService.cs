using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.Autofac.Test
{
    public class SomeService : ISomeService
    {
        public void DoSomething()
        {
            Console.WriteLine("DI-Succes");
        }
    }
}
