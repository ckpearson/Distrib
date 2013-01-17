using Distrib.Utils;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginControllerValidationService : IPluginControllerValidationService
    {
        public Res<Type, PluginControllerValidationResult> ValidateControllerType(Type controllerType)
        {
            var res = PluginControllerValidationResult.Success;

            if (controllerType == null) throw new ArgumentNullException("Controller type must be supplied");

            try
            {
                res = CChain<PluginControllerValidationResult>
                    // Must be a class
                    .If(() => !controllerType.IsClass, PluginControllerValidationResult.ControllerTypeNotAClass)
                    // Must be marshalable
                    .ThenIf(() => controllerType.BaseType == null || controllerType.BaseType != typeof(CrossAppDomainObject),
                        PluginControllerValidationResult.ControllerTypeNotMarshalable)
                    // Must implement the core controller interface
                    .ThenIf(() => controllerType.GetInterface(typeof(IPluginController).FullName) == null,
                        PluginControllerValidationResult.ControllerInterfaceNotImplemented)
                    // Must have a constructor taking an IKernel
                    //.ThenIf(() => controllerType.GetConstructor(new[] { typeof(IKernel) }) == null,
                    //    PluginControllerValidationResult.KernelAcceptingConstructorNotFound)
                    .Result;

                return new Res<Type, PluginControllerValidationResult>(res == PluginControllerValidationResult.Success,
                    controllerType,
                    res);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to validate controller type", ex);
            }
        }
    }
}
