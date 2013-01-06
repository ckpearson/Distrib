using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class IOCAttribute : Attribute
    {
        private readonly bool _forIOC;

        public IOCAttribute(bool forIOC)
        {
            _forIOC = forIOC;
        }

        public bool ForIOC
        {
            get { return _forIOC; }
        }
    }
}
