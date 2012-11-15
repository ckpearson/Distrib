using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Controllers
{
    internal sealed class DistribDefaultPluginController : IDistribPluginController
    {
    }

    internal enum DistribPluginControllerValidationResult
    {
        Success = 0,
        UnknownFailure,
        ControllerTypeNotAClass,
        ControllerInterfaceNotImplemented,
    }

    internal static class DistribPluginControllerSystem
    {

        public static Res<Type, DistribPluginControllerValidationResult> ValidateAndReturnControllerType(Type controllerType)
        {
            var res = DistribPluginControllerValidationResult.Success;

            if (controllerType == null) throw new ArgumentNullException("Controller type must be supplied");

            try
            {
                if (!controllerType.IsClass)
                {
                    res = DistribPluginControllerValidationResult.ControllerTypeNotAClass;
                }

                if (controllerType.GetInterface(typeof(IDistribPluginController).FullName) == null)
                {
                    res = DistribPluginControllerValidationResult.ControllerInterfaceNotImplemented;
                }

                return new Res<Type, DistribPluginControllerValidationResult>(res == DistribPluginControllerValidationResult.Success,
                    controllerType, res);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to validate and return controller type", ex);
            }
        }

        public static Res<Type, DistribPluginControllerValidationResult> ValidateAndReturnControllerType<T>()
        {
            return ValidateAndReturnControllerType(typeof(T));
        }
    }

    public interface IDistribPluginController
    {

    }
}
