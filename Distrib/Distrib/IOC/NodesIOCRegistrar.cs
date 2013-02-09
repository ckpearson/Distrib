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
            BindSingleton<Nodes.Process.IProcessNodeFactory,
                Nodes.Process.ProcessNodeFactory>();

            Bind<Nodes.Process.IProcessNode,
                Nodes.Process.StandardProcessNode>();
        }
    }
}
