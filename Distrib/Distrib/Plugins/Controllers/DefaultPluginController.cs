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

    internal static class DistribPluginControllerSystem
    {
        public static Type ValidateAndReturnControllerType(Type controllerType)
        {
            if (controllerType == null) throw new ArgumentNullException("controller type must be supplied");

            try
            {

                return controllerType;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to validate and return controller type", ex);
            }
        }

        public static Type ValidateAndReturnControllerType<T>()
        {
            return ValidateAndReturnControllerType(typeof(T));
        }
    }

    public interface IDistribPluginController
    {

    }
}
