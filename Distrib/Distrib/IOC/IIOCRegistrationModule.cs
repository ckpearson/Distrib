using Distrib.IOC.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC.Interface
{
    /// <summary>
    /// IOC registration module interface
    /// </summary>
    public interface IIOCRegistrationModule
    {
        /// <summary>
        /// Perform all the bindings for the module
        /// </summary>
        /// <param name="bindAction">The action to invoke that performs the binding from type to type</param>
        void PerformBindings(Action<Type, Type> bindAction);
    }
}
