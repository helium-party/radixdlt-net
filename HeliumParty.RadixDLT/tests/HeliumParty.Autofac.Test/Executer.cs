using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.Autofac.Test
{
    public class Executer
    {
        private readonly ISomeService _service;

        public Executer(ISomeService service)
        {
            _service = service;
        }

        public void Run()
        {
            _service.DoSomething();
        }
    }
}
