using Distrib.Nodes.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public sealed class NodesIOCRegistrar : IOCRegistrar
    {
        public override void PerformBindings()
        {
            BindSingleton<IProcessNodeFactory, ProcessNodeFactory>();
            Bind<IProcessNode, StandardProcessNode>();
        }
    }
}
