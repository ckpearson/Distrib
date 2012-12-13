using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginControllerFactory : IPluginControllerFactory
    {
        private IKernel _kernel;

        public PluginControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginController CreateController()
        {
            return _kernel.Get<IPluginController>();
        }

        public IPluginController CreateControllerOfType(Type type, IPluginControllerValidationService controllerValidator)
        {
            if (type == null) throw new ArgumentNullException("type must be supplied");

            try
            {
                var controllerValidationResult = controllerValidator.ValidateControllerType(type);

                if (!controllerValidationResult.Success)
                {
                    throw new InvalidOperationException(string.Format("Cannot use type to create controller, type failed validation: {0}",
                        controllerValidationResult.ResultTwo.ToString()));
                }
                else
                {
                    return (IPluginController)Activator.CreateInstance(type);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create controller of given type", ex);
            }
        }
    }
}
