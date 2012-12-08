using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public interface IRemoteKernelFactory
    {
        IRemoteKernel GetRemoteKernel(IKernel kernel);
    }
}
