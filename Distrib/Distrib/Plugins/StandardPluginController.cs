/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
using Distrib.Utils;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class StandardPluginController : CrossAppDomainObject, IPluginController
    {
        private WriteOnce<RemoteAppDomainBridge> _appDomainBridge =
            new WriteOnce<RemoteAppDomainBridge>(null);

        private WriteOnce<IPluginDescriptor> _pluginDescriptor =
            new WriteOnce<IPluginDescriptor>(null);

        private WriteOnce<IPlugin> _pluginInstance =
            new WriteOnce<IPlugin>(null);

        private WriteOnce<IPluginInteractionLink> _pluginInteractionLink = new WriteOnce<IPluginInteractionLink>(null);

        private readonly object _lock = new object();

        public StandardPluginController()
        {
        }

        public void TakeRemoteBridge(RemoteAppDomainBridge bridge)
        {
            lock (_lock)
            {
                if (!_appDomainBridge.IsWritten)
                {
                    _appDomainBridge.Value = bridge;
                }
                else
                {
                    throw new InvalidOperationException("Bridge already supplied");
                }
            }
        }

        public object CreatePluginInstance(IPluginDescriptor descriptor, string pluginAssemblyPath,
            IPluginInstance pluginManagedInstance,
            IPluginInteractionLinkFactory pluginInteractionLinkFactory)
        {
            if (descriptor == null) throw new ArgumentNullException("Plugin descriptor must be supplied");
            if (string.IsNullOrEmpty(pluginAssemblyPath)) throw new ArgumentNullException("Plugin assembly path must be supplied");

            try
            {
                lock (_lock)
                {
                    if (!_pluginInstance.IsWritten)
                    {
                        _pluginDescriptor.Value = descriptor;

                        _pluginInstance.Value = (IPlugin)_appDomainBridge.Value.CreateInstance(descriptor.PluginTypeName,
                            pluginAssemblyPath);

                        _pluginInteractionLink.Value = pluginInteractionLinkFactory.CreateInteractionLink(
                            descriptor,
                            _pluginInstance.Value,
                            this,
                            pluginManagedInstance);

                        return _pluginInstance.Value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Controller already created plugin instance");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin instance", ex);
            }
        }

        public IPluginInteractionLink InteractionLink
        {
            get
            {
                lock (_lock)
                {
                    if (!_pluginInteractionLink.IsWritten)
                    {
                        return null;
                    }

                    return _pluginInteractionLink.Value;
                }
            }
        }

        public void InitController()
        {
            // Controller initialisation
        }

        public void UninitController()
        {
            // Controller uninitialisation (not to unitialise instance here)
        }

        public void InitialiseInstance()
        {
            if (_pluginInstance.IsWritten)
            {
                _pluginInstance.Value.InitialisePlugin(_pluginInteractionLink.Value);
            }
            else
            {
                throw new InvalidOperationException("Controller doesn't hold an instance yet");
            }
        }

        public void UnitialiseInstance()
        {
            if (_pluginInstance.IsWritten)
            {
                _pluginInstance.Value.UninitialisePlugin(_pluginInteractionLink.Value);
            }
            else
            {
                throw new InvalidProgramException("Controller doesn't hold an instance yet");
            }
        }
    }
}
