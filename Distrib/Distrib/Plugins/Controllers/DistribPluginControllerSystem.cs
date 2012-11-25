using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Controllers
{
    /// <summary>
    /// Core system provider for the plugin controllers
    /// </summary>
    internal static class DistribPluginControllerSystem
    {
        /// <summary>
        /// Validates the given controller type
        /// </summary>
        /// <param name="controllerType">The controller type to validate</param>
        /// <returns>The validation result</returns>
        public static Res<Type, DistribPluginControllerValidationResult> ValidateControllerType(Type controllerType)
        {
            var res = DistribPluginControllerValidationResult.Success;

            if (controllerType == null) throw new ArgumentNullException("Controller type must be supplied");

            try
            {
                res = CChain<DistribPluginControllerValidationResult>
                    // Must be a class
                    .If(() => !controllerType.IsClass, DistribPluginControllerValidationResult.ControllerTypeNotAClass)
                    .ThenIf(() => controllerType.BaseType == null || controllerType.BaseType != typeof(MarshalByRefObject),
                        DistribPluginControllerValidationResult.ControllerTypeNotMarshalable)
                    // Must implement the interface
                    .ThenIf(() => controllerType.GetInterface(typeof(IDistribPluginController).FullName) == null, 
                        DistribPluginControllerValidationResult.ControllerInterfaceNotImplemented)
                    // Must have a public parameterless constructor
                    .ThenIf(() => controllerType.GetConstructor(Type.EmptyTypes) == null, 
                        DistribPluginControllerValidationResult.ControllerTypeMissingPublicParameterlessConstructor)
                    .Result;

                return new Res<Type, DistribPluginControllerValidationResult>(res == DistribPluginControllerValidationResult.Success,
                    controllerType, res);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to validate and return controller type", ex);
            }
        }

        /// <summary>
        /// Validates the given controller type
        /// </summary>
        /// <typeparam name="T">The controller type</typeparam>
        /// <returns>The validation result</returns>
        public static Res<Type, DistribPluginControllerValidationResult> ValidateControllerType<T>()
        {
            return ValidateControllerType(typeof(T));
        }
    }
}
