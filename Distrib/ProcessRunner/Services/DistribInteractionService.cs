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
using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Processes;
using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Services
{
    [Export(typeof(IDistribInteractionService))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public sealed class DistribInteractionService : IDistribInteractionService
    {
        private IIOC _distribIOC;
        private IEventAggregator _eventAggregator;
        private IAppStateService _appState;

        [ImportingConstructor()]
        public DistribInteractionService(IEventAggregator eventAggregator, IAppStateService appState)
        {
            _eventAggregator = eventAggregator;
            _appState = appState;
            _distribIOC = new Distrib.IOC.Ninject.NinjectBootstrapper();
            _distribIOC.Start();
        }

        private Models.PluginAssembly _currentPluginAssembly;

        public void LoadAssembly(string assemblyPath)
        {
            if (_currentPluginAssembly != null)
            {
                // Unload the existing assembly first
                _currentPluginAssembly = null;
            }

            _currentPluginAssembly = new Models.PluginAssembly(assemblyPath,
                _distribIOC.Get<IPluginAssemblyFactory>());
            _eventAggregator.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                .Publish(Events.PluginAssemblyStateChange.AssemblyLoaded);

            // Now initialise the assembly
            _currentPluginAssembly.Init();
            _eventAggregator.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                .Publish(Events.PluginAssemblyStateChange.AssemblyInitialised);
        }


        public Models.PluginAssembly CurrentAssembly
        {
            get { return _currentPluginAssembly; }
        }

        public void UnloadAssembly()
        {
            if (_currentPluginAssembly == null)
            {
                throw new InvalidOperationException();
            }

            // Uninitialise if need to first
            if (_currentPluginAssembly.Initialised)
            {
                _currentPluginAssembly.Uninit();
                _eventAggregator.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                    .Publish(Events.PluginAssemblyStateChange.AssemblyUninitialised);
            }

            // Remove reference
            _currentPluginAssembly = null;

            _eventAggregator.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                .Publish(Events.PluginAssemblyStateChange.AssemblyUnloaded);
        }


        public Distrib.Processes.IProcessHost CreateProcessHost(Models.DistribProcess process)
        {
            if (CurrentAssembly == null || !CurrentAssembly.Initialised)
            {
                throw new InvalidOperationException();
            }

            try
            {
                if (!process.Plugin.IsUsable)
                {
                    throw new InvalidOperationException("The process isn't usable");
                }

                var procHost = _distribIOC.Get<IProcessHostFactory>()
                    .CreateHostFromPlugin(process.Plugin.RawDescriptor);
                procHost.Initialise();

                return procHost;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create process host", ex);
            }
        }
    }
}
