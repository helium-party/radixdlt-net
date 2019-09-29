using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.DependencyInjection
{
    public class DependencyAttribute : Attribute
    {
        public virtual bool TryRegister { get; set; }

        public virtual bool ReplaceServices { get; set; }

        public DependencyAttribute()
        {

        }
    }
}
