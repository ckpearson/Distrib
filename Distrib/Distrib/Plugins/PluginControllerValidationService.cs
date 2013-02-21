/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using Distrib.Utils;
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
