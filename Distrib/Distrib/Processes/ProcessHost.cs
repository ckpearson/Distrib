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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class ProcessHost : CrossAppDomainObject, IProcessHost, IJobInputTracker, IJobOutputTracker
    {
        private readonly IPluginDescriptor _descriptor;
        private readonly IPluginAssemblyFactory _assemblyFactory;

        private bool _isInitialised = false;

        private readonly object _lock = new object();

        private IPluginAssembly _pluginAssembly;

        private IPluginInstance _pluginInstance;

        private IProcess _processInstance;

        private readonly IJobFactory _jobFactory;

        private WriteOnce<IJobDescriptor> _jobDescriptor = new WriteOnce<IJobDescriptor>(null);
        private readonly IJobDescriptorFactory _jobDescriptorFactory;

        public ProcessHost([IOC(false)] IPluginDescriptor descriptor, [IOC(true)] IPluginAssemblyFactory assemblyFactory,
            [IOC(true)] IJobFactory jobFactory,
            [IOC(true)] IJobDescriptorFactory jobDescriptorFactory)
        {
            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");
            if (assemblyFactory == null) throw new ArgumentNullException("Assembly factory must be supplied");
            if (jobFactory == null) throw Ex.ArgNull(() => jobFactory);

            try
            {
                if (!descriptor.IsUsable)
                {
                    throw new InvalidOperationException("Descriptor must be for a usable plugin");
                }

                if (!descriptor.Metadata.InterfaceType.Equals(typeof(IProcess)))
                {
                    throw new InvalidOperationException("Descriptor must be for a process plugin");
                }

                _descriptor = descriptor;
                _assemblyFactory = assemblyFactory;
                _jobFactory = jobFactory;
                _jobDescriptorFactory = jobDescriptorFactory;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to construct instance", ex);
            }
        }

        public void Initialise()
        {
            try
            {
                lock (_lock)
                {
                    if (IsInitialised)
                    {
                        throw new InvalidOperationException("Already initialised");
                    }

                    // Load the plugin .NET assembly itself into the AppDomain
                    System.Reflection.Assembly.LoadFrom(_descriptor.AssemblyPath);

                    // Hook into the assembly resolve so any types can be brought back over the instance
                    // domain
                    AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                        {
                            // Look in the current AppDomain for it, if it's not there (which it ought to be)
                            // then a null will result in an exception when resolving
                            return AppDomain.CurrentDomain.GetAssemblies()
                                .DefaultIfEmpty(null)
                                .SingleOrDefault(asm => asm.FullName == e.Name);
                        };

                    _pluginAssembly = _assemblyFactory.CreatePluginAssemblyFromPath(_descriptor.AssemblyPath);
                    var res = _pluginAssembly.Initialise();
                    if (!res.HasUsablePlugins)
                    {
                        throw new InvalidOperationException("Plugin assembly contains no usable plugins");
                    }

                    if (res.UsablePlugins.DefaultIfEmpty(null)
                        .SingleOrDefault(p => p.Match(_descriptor)) == null)
                    {
                        throw new InvalidOperationException("A matching plugin descriptor could not be found in the plugin assembly");
                    }

                    _pluginInstance = _pluginAssembly.CreatePluginInstance(_descriptor);

                    _pluginInstance.Initialise();

                    _processInstance = _pluginInstance.GetUnderlyingInstance<IProcess>();
                    _processInstance.InitProcess();

                    // Ensure the process actually has some jobs
                    if (_processInstance.JobDefinitions == null || _processInstance.JobDefinitions.Count == 0)
                    {
                        throw new ApplicationException("Process doesn't have any job definitions");
                    }

                    _isInitialised = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise", ex);
            }
        }

        public void Unitialise()
        {
            try
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        throw new InvalidOperationException("Not initialised");
                    }

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
                throw new ApplicationException("Failed to unitialise", ex);
            }
        }


        public bool IsInitialised
        {
            get
            {
                lock (_lock)
                {
                    return _isInitialised;
                }
            }
        }

        T IJobInputTracker.GetInput<T>(IJob forJob, string prop)
        {
            if (string.IsNullOrEmpty(prop))
            {
                throw new InvalidOperationException("input property name must be supplied");
            }

            var internalJob = forJob as IJob_Internal;

            var jd = internalJob.Definition;


            // Check the definition contains an input field by that name
            var inputDefField = jd.InputFields.SingleOrDefault(f => f.Name == prop);

            if (inputDefField == null)
            {
                throw new InvalidOperationException("No input field could be found for that input property");
            }

            // Check to see if the input has already been bundled with the job definition / already asked for and cached
            var inputValueField = internalJob.InputValueFields.SingleOrDefault(f => f.Definition.Name == prop);

            if (inputValueField != null && inputValueField.Value != null)
            {
                // The job already has that information
                return (T)inputValueField.Value;
            }
            else
            {
                // The job doesn't have the value data for this field, this is where any retrieval of lazy-retrieve values would be done
                // it's also where any sort of default value things could be done too

                if (inputDefField.Config.HasDefaultValue)
                {
                    var value = inputDefField.Config.DefaultValue;

                    // Need to shove this in as an input field so it's cached and ready to go

                    internalJob.SetInputValue(inputDefField, value);

                    return (T)value;
                }
                else
                {
                    if (!inputDefField.Config.HasDeferredValueProvider)
                    {
                        throw new InvalidOperationException(string.Format("No value could be found for input '{0}' and it has no default",
                            inputDefField.Name));
                    }
                    else
                    {
                        return (T)inputDefField.Config.DeferredValueProvider.RetrieveValue();
                    }
                }
            }
        }

        T IJobOutputTracker.GetOutput<T>(IJob forJob, string prop)
        {
            if (string.IsNullOrEmpty(prop))
            {
                throw new InvalidOperationException("input property name must be supplied");
            }

            var internalJob = forJob as IJob_Internal;

            var jd = internalJob.Definition;


            // Check the definition contains an input field by that name
            var outputDefField = jd.OutputFields.SingleOrDefault(f => f.Name == prop);

            if (outputDefField == null)
            {
                throw new InvalidOperationException("No input field could be found for that input property");
            }

            // Check to see if the input has already been bundled with the job definition / already asked for and cached
            var outputValueField = internalJob.OutputValueFields.SingleOrDefault(f => f.Definition.Name == prop);

            if (outputValueField != null)
            {
                // The job already has that information
                return (T)outputValueField.Value;
            }
            else
            {
                // The job doesn't have the value data for this field, this is where any retrieval of lazy-retrieve values would be done
                // it's also where any sort of default value things could be done too

                if (outputDefField.Config.HasDefaultValue)
                {
                    var value = outputDefField.Config.DefaultValue;

                    // Need to shove this in as an input field so it's cached and ready to go

                    internalJob.SetInputValue(outputDefField, value);

                    return (T)value;
                }
                else
                {
                    throw new InvalidOperationException("No value could be retrieved for this input field");
                }
            }
        }

        void IJobOutputTracker.SetOutput<T>(IJob forJob, T value, string prop)
        {
            if (string.IsNullOrEmpty(prop))
            {
                throw new InvalidOperationException("input property name must be supplied");
            }

            var internalJob = forJob as IJob_Internal;

            var jd = forJob.Definition;


            // Check the definition contains an input field by that name
            var outputDefField = jd.OutputFields.SingleOrDefault(f => f.Name == prop);

            if (outputDefField == null)
            {
                throw new InvalidOperationException("No input field could be found for that input property");
            }

            // Check to see if the input has already been bundled with the job definition / already asked for and cached
            var outputValueField = internalJob.OutputValueFields.SingleOrDefault(f => f.Definition.Name == prop);

            if (outputValueField != null)
            {
                // The output for that has already been set, just overrite it
                outputValueField.Value = value;
            }
            else
            {
                internalJob.SetOutputValue(outputDefField, value);
            }
        }

        public IEnumerable<IProcessJobValueField> ProcessJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputFields = null)
        {
            try
            {
                IJob job = _jobFactory.CreateJob(this, this, definition);
                var jobInternal = ((IJob_Internal)job);

                if (inputFields != null)
                {
                    foreach (var infield in inputFields)
                    {
                        jobInternal.SetInputValue(infield.Definition, infield.Value);
                    }
                }

                _processInstance.ProcessJob(job);

                var outValues = jobInternal.OutputValueFields;

                if (outValues == null)
                {
                    outValues = new List<IProcessJobValueField>();
                }

                foreach (var defOutField in jobInternal.Definition.OutputFields)
                {
                    var matchValField = outValues.SingleOrDefault(f => f.Definition.Name == defOutField.Name);

                    if (matchValField != null)
                    {
                        // The value field was found, so it's been set by the process
                        continue;
                    }
                    else
                    {
                        // The output hasn't been set, it may have a default value
                        if (defOutField.Config.HasDefaultValue)
                        {
                            jobInternal.SetOutputValue(defOutField, defOutField.Config.DefaultValue);
                            continue;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("The process didn't set the output '{0}' and no default is configured",
                                defOutField.Name));
                        }
                    }
                }

                return jobInternal.OutputValueFields;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to process job", ex);
            }
        }

        public IPluginDescriptor PluginDescriptor
        {
            get { return _descriptor; }
        }


        public Task<IEnumerable<IProcessJobValueField>> ProcessJobAsync(IEnumerable<IProcessJobValueField> inputValues = null)
        {
            //return Task<IEnumerable<IProcessJobValueField>>.Factory.StartNew(() =>
            //    {
            //        return ProcessJob(inputValues);
            //    });

            throw new NotImplementedException();
        }


        public IReadOnlyList<string> RegisteredPluginDependencyAssemblies
        {
            get
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        return null;
                    }

                    return _pluginInstance.DeclaredAssemblyDependencies;
                }
            }
        }


        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get
            {
                return _processInstance.JobDefinitions;
            }
        }


        public DateTime InstanceCreationStamp
        {
            get
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        return DateTime.MinValue;
                    }

                    return _pluginInstance.InstanceCreationStamp;
                }
            }
        }


        public string InstanceID
        {
            get
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        return null;
                    }

                    return _pluginInstance.InstanceID;
                }
            }
        }
    }
}
