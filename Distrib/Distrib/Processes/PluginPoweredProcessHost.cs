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
using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Separation;
using Distrib.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class PluginPoweredProcessHost
        : ProcessHostBase,
        IPluginPoweredProcessHost
    {
        private readonly IPluginDescriptor _pluginDescriptor;
        private IPluginAssembly _pluginAssembly;
        private IPluginAssemblyFactory _pluginAssemblyFactory;

        private IPluginInstance _pluginInstance;

        public PluginPoweredProcessHost(
            [IOC(false)] IPluginDescriptor descriptor,
            [IOC(true)] IPluginAssemblyFactory assemblyFactory,
            [IOC(true)] IJobFactory jobFactory)
            : base(jobFactory)
        {
            if (descriptor == null) throw Ex.ArgNull(() => descriptor);
            if (assemblyFactory == null) throw Ex.ArgNull(() => assemblyFactory);
            if (jobFactory == null) throw Ex.ArgNull(() => jobFactory);

            try
            {
                if (!descriptor.IsUsable)
                {
                    throw Ex.Arg(() => descriptor, "Descriptor must be for a usable plugin");
                }

                if (!descriptor.Metadata.InterfaceType.Equals(typeof(IProcess)))
                {
                    throw Ex.Arg(() => descriptor, "Descriptor must be for a process plugin");
                }

                _pluginDescriptor = descriptor;
                _pluginAssemblyFactory = assemblyFactory;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin powered process host", ex);
            }
        }

        protected override void DoInit()
        {
            try
            {
                lock (_lock)
                {
                    Assembly.LoadFrom(_pluginDescriptor.AssemblyPath);

                    AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                        {
                            return AppDomain.CurrentDomain.GetAssemblies()
                                .DefaultIfEmpty(null)
                                .SingleOrDefault(asm => asm.FullName == e.Name);
                        };

                    try
                    {
                        _pluginAssembly = _pluginAssemblyFactory.CreatePluginAssemblyFromPath(_pluginDescriptor.AssemblyPath);
                        var initRes = _pluginAssembly.Initialise();

                        if (!initRes.HasUsablePlugins)
                        {
                            throw new ApplicationException("The plugin assembly contains no usable plugins");
                        }

                        if (initRes.UsablePlugins.DefaultIfEmpty(null)
                            .SingleOrDefault(p => p.Match(_pluginDescriptor)) == null)
                        {
                            throw new ApplicationException("A matching plugin descriptor couldn't be found in the plugin assembly");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to initialise plugin assembly and locate plugin", ex);
                    }

                    try
                    {
                        _pluginInstance = _pluginAssembly.CreatePluginInstance(_pluginDescriptor);
                        _pluginInstance.Initialise();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to create and initialise the plugin instance", ex);
                    }

                    try
                    {
                        _processInstance = _pluginInstance.GetUnderlyingInstance<IProcess>();
                        _processInstance.InitProcess();

                        if (_processInstance.JobDefinitions == null || _processInstance.JobDefinitions.Count == 0)
                        {
                            throw new ApplicationException("Process doesn't have any job definitions");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to create and initialise the process", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to perform init", ex);
            }
        }

        protected override void DoUninit()
        {
            try
            {
                lock (_lock)
                {
                    if (_processInstance != null)
                    {
                        _processInstance.UninitProcess();
                    }

                    if (_pluginAssembly != null && _pluginAssembly.IsInitialised)
                    {
                        _pluginAssembly.Unitialise();
                    }

                    _pluginAssembly = null;
                    _pluginInstance = null;
                    _processInstance = null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to do uninit", ex);
            }
        }

        protected override DateTime GetInstanceCreationStamp()
        {
            lock (_lock)
            {
                if (!IsInitialised)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return _pluginInstance.InstanceCreationStamp;
                }
            }
        }

        protected override string GetInstanceID()
        {
            lock (_lock)
            {
                if (!IsInitialised)
                {
                    return null;
                }
                else
                {
                    return _pluginInstance.InstanceID;
                }
            }
        }

        public IPluginDescriptor PluginDescriptor
        {
            get { return _pluginDescriptor; }
        }

        public IReadOnlyList<string> DeclaredDependentAssemblies
        {
            get
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        return null;
                    }
                    else
                    {
                        return _pluginInstance.DeclaredAssemblyDependencies;
                    }
                }
            }
        }
    }


}
