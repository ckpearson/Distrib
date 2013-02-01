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

            if (!process.Plugin.IsUsable)
            {
                throw new InvalidOperationException();
            }

            var procHost = _distribIOC.Get<IProcessHostFactory>()
                .CreateHostFromPluginSeparated(process.Plugin.RawDescriptor);
            procHost.Initialise();

            return procHost;
        }
    }
}
