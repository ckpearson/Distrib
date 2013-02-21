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
    public sealed class PluginBootstrapService : IPluginBootstrapService
    {
        private readonly IPluginControllerFactory _pluginControllerFactory;

        public PluginBootstrapService(IPluginControllerFactory pluginControllerFactory)
        {
            _pluginControllerFactory = pluginControllerFactory;
        }

        public Res<PluginBootstrapResult> BootstrapPlugin(IPluginDescriptor descriptor)
        {
            PluginBootstrapResult res = PluginBootstrapResult.Success;

            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");

            try
            {
                // If descriptor has no controller type set, then set the current default
                if (descriptor.Metadata.ControllerType == null)
                {
                    descriptor.Metadata.ControllerType = _pluginControllerFactory
                        .CreateController().GetType();
                }

                return new Res<PluginBootstrapResult>(res == PluginBootstrapResult.Success, res);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to bootstrap plugin", ex);
            }
        }
    }
}
