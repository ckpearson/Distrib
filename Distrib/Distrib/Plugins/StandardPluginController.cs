using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class StandardPluginController : MarshalByRefObject, IPluginController
    {
        private IKernel _kernel;

        public StandardPluginController(IKernel kernel)
        {
            _kernel = kernel;
        }
    }
}
