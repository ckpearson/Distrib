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
    public sealed class PluginPoweredProcessHost : CrossAppDomainObject
        , IProcessHost
        , IPluginPoweredProcessHost
        , IJobInputTracker
        , IJobOutputTracker
    {
        private readonly object _lock = new object();

        private IPluginDescriptor _pluginDescriptor;

        private IPluginAssembly _pluginAssembly;
        private IPluginAssemblyFactory _pluginAssemblyFactory;

        private IJobFactory _jobFactory;

        private IPluginInstance _pluginInstance;
        private IProcess _processInstance;

        private bool _isInitialised = false;

        public PluginPoweredProcessHost(
            [IOC(false)] IPluginDescriptor descriptor,
            [IOC(true)] IPluginAssemblyFactory assemblyFactory,
            [IOC(true)] IJobFactory jobFactory)
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
                _jobFactory = jobFactory;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin powered process host", ex);
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
                        throw new InvalidOperationException("Process host is already initialised");
                    }

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

                _isInitialised = true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise plugin powered process host", ex);
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
                        throw new InvalidOperationException("Process host not initialised");
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

                    _isInitialised = false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to uninitialise plugin powered process host", ex);
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

        public IReadOnlyList<IJobDefinition> JobDefinitions
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
                        return _processInstance.JobDefinitions;
                    }
                }
            }
        }

        public IReadOnlyList<IProcessJobValueField> ProcessJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            try
            {
                IJob job = _jobFactory.CreateJob(this, this, definition);
                IJob_Internal jobInternal = (IJob_Internal)job;

                try
                {
                    if (inputValues != null)
                    {
                        foreach (var inField in inputValues)
                        {
                            jobInternal.SetInputValue(inField.Definition, inField.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to set input values on job", ex);
                }

                try
                {
                    _processInstance.ProcessJob(job);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Process instance failed to process the job", ex);
                }

                try
                {
                    var outValues = jobInternal.OutputValueFields != null ? jobInternal.OutputValueFields : new List<IProcessJobValueField>();

                    foreach (var defOutField in jobInternal.Definition.OutputFields)
                    {
                        var matchValField = outValues.SingleOrDefault(f => f.Definition.Name == defOutField.Name);

                        if (matchValField != null)
                        {
                            continue;
                        }
                        else
                        {
                            if (defOutField.Config.HasDefaultValue)
                            {
                                jobInternal.SetOutputValue(defOutField, defOutField.Config.DefaultValue);
                                continue;
                            }
                            else
                            {
                                throw new ApplicationException(string.Format("The process didn't set the output '{0}' and no default is configured",
                                    defOutField.Name));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to retrieve the output value fields after job processing", ex);
                }

                return jobInternal.OutputValueFields;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Plugin powered process host failed to process job", ex);
            }
        }

        public Task<IReadOnlyList<IProcessJobValueField>>
            ProcessJobAsync(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            return new Task<IReadOnlyList<IProcessJobValueField>>(() =>
            {
                return ProcessJob(definition, inputValues);
            });
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
                    else
                    {
                        return _pluginInstance.InstanceCreationStamp;
                    }
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
                    else
                    {
                        return _pluginInstance.InstanceID;
                    }
                }
            }
        }

        public IPluginDescriptor PluginDescriptor
        {
            get
            {
                lock (_lock)
                {
                    return _pluginDescriptor;
                }
            }
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

        public T GetInput<T>(IJob forJob, string prop = null)
        {
            if (forJob == null) Ex.ArgNull(() => forJob);
            if (string.IsNullOrEmpty(prop)) Ex.ArgNull(() => prop); // [CallerMemberName]

            try
            {
                var internalJob = (IJob_Internal)forJob;
                var definition = internalJob.Definition;

                var inputDefField = definition.InputFields.SingleOrDefault(f => f.Name == prop);

                if (inputDefField == null)
                {
                    throw new ApplicationException(string.Format("No input field could be found '{0}'", prop));
                }

                var inputValField = internalJob.InputValueFields.SingleOrDefault(f => f.Definition.Name == prop);

                if (inputValField != null && inputValField.Value != null)
                {
                    // The value has already been set (it may have been requested before or set prior to processing)
                    return (T)inputValField.Value;
                }
                else
                {
                    if (inputDefField.Config.HasDefaultValue)
                    {
                        // A default value has been configured to set and return
                        internalJob.SetInputValue(inputDefField, inputDefField.Config.DefaultValue);
                        return (T)inputDefField.Config.DefaultValue;
                    }
                    else
                    {
                        if (inputDefField.Config.HasDeferredValueProvider)
                        {
                            // A value provider has been configured so ask that for the value
                            return (T)inputDefField.Config.DeferredValueProvider.RetrieveValue();
                        }
                        else
                        {
                            // No value could be determined
                            throw new ApplicationException(string.Format("No value could be found for input '{0}' and it has no " +
                                "alternatives configured", inputDefField.Name));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get input", ex);
            }
        }

        public T GetOutput<T>(IJob forJob, string prop = null)
        {
            if (forJob == null) Ex.ArgNull(() => forJob);
            if (string.IsNullOrEmpty(prop)) Ex.ArgNull(() => prop);

            try
            {
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
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get output", ex);
            }
        }

        public void SetOutput<T>(IJob forJob, T value, string prop = null)
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
    }

    public sealed class TypePoweredProcessHost : CrossAppDomainObject,
        IProcessHost,
        ITypePoweredProcessHost,
        IJobInputTracker,
        IJobOutputTracker
    {
        private readonly Type _instanceType;
        private IProcess _processInstance;

        private readonly IJobFactory _jobFactory;

        private readonly object _lock = new object();

        private bool _isInitialised = false;

        private DateTime _creationStamp;
        private string _creationID;

        public TypePoweredProcessHost(
            [IOC(false)] Type instanceType,
            [IOC(true)] IJobFactory jobFactory)
        {
            if (instanceType == null) throw Ex.ArgNull(() => instanceType);
            if (jobFactory == null) throw Ex.ArgNull(() => jobFactory);

            if (!instanceType.IsClass)
            {
                throw Ex.Arg(() => instanceType, "Instance type must be a class");
            }

            if (instanceType.GetInterface(typeof(IProcess).FullName) == null)
            {
                throw Ex.Arg(() => instanceType, "Instance type must implement IProcess");
            }

            if (instanceType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw Ex.Arg(() => instanceType, "Instance type must have a public parameterless constructor");
            }

            _instanceType = instanceType;
            _jobFactory = jobFactory;
        }

        public void Initialise()
        {
            try
            {
                lock (_lock)
                {
                    if (IsInitialised)
                    {
                        throw new InvalidOperationException("Process host already initialised");
                    }

                    try
                    {
                        _processInstance = (IProcess)Activator.CreateInstance(_instanceType);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to create process instance", ex);
                    }

                    _creationID = Guid.NewGuid().ToString();
                    _creationStamp = DateTime.Now;

                    _isInitialised = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise instance powered process host", ex);
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
                        throw new InvalidOperationException("Process host not initialised");
                    }

                    if (_processInstance != null)
                    {
                        _processInstance.UninitProcess();
                    }

                    _processInstance = null;
                    _isInitialised = false;
                    _creationStamp = DateTime.MinValue;
                    _creationID = null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to uninitialise instance powered process host", ex);
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

        public IReadOnlyList<IJobDefinition> JobDefinitions
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
                        return _processInstance.JobDefinitions;
                    }
                }
            }
        }

        public IReadOnlyList<IProcessJobValueField> ProcessJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            try
            {
                IJob job = _jobFactory.CreateJob(this, this, definition);
                IJob_Internal jobInternal = (IJob_Internal)job;

                try
                {
                    if (inputValues != null)
                    {
                        foreach (var inField in inputValues)
                        {
                            jobInternal.SetInputValue(inField.Definition, inField.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to set input values on job", ex);
                }

                try
                {
                    _processInstance.ProcessJob(job);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Process instance failed to process the job", ex);
                }

                try
                {
                    var outValues = jobInternal.OutputValueFields != null ? jobInternal.OutputValueFields : new List<IProcessJobValueField>();

                    foreach (var defOutField in jobInternal.Definition.OutputFields)
                    {
                        var matchValField = outValues.SingleOrDefault(f => f.Definition.Name == defOutField.Name);

                        if (matchValField != null)
                        {
                            continue;
                        }
                        else
                        {
                            if (defOutField.Config.HasDefaultValue)
                            {
                                jobInternal.SetOutputValue(defOutField, defOutField.Config.DefaultValue);
                                continue;
                            }
                            else
                            {
                                throw new ApplicationException(string.Format("The process didn't set the output '{0}' and no default is configured",
                                    defOutField.Name));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to retrieve the output value fields after job processing", ex);
                }

                return jobInternal.OutputValueFields;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Instance powered process host failed to process job", ex);
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
                    else
                    {
                        return _creationStamp;
                    }
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
                    else
                    {
                        return _creationID;
                    }
                }
            }
        }

        public Type InstanceType
        {
            get { return _instanceType; }
        }

        public T GetInput<T>(IJob forJob, string prop = null)
        {
            if (forJob == null) Ex.ArgNull(() => forJob);
            if (string.IsNullOrEmpty(prop)) Ex.ArgNull(() => prop); // [CallerMemberName]

            try
            {
                var internalJob = (IJob_Internal)forJob;
                var definition = internalJob.Definition;

                var inputDefField = definition.InputFields.SingleOrDefault(f => f.Name == prop);

                if (inputDefField == null)
                {
                    throw new ApplicationException(string.Format("No input field could be found '{0}'", prop));
                }

                var inputValField = internalJob.InputValueFields.SingleOrDefault(f => f.Definition.Name == prop);

                if (inputValField != null && inputValField.Value != null)
                {
                    // The value has already been set (it may have been requested before or set prior to processing)
                    return (T)inputValField.Value;
                }
                else
                {
                    if (inputDefField.Config.HasDefaultValue)
                    {
                        // A default value has been configured to set and return
                        internalJob.SetInputValue(inputDefField, inputDefField.Config.DefaultValue);
                        return (T)inputDefField.Config.DefaultValue;
                    }
                    else
                    {
                        if (inputDefField.Config.HasDeferredValueProvider)
                        {
                            // A value provider has been configured so ask that for the value
                            return (T)inputDefField.Config.DeferredValueProvider.RetrieveValue();
                        }
                        else
                        {
                            // No value could be determined
                            throw new ApplicationException(string.Format("No value could be found for input '{0}' and it has no " +
                                "alternatives configured", inputDefField.Name));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get input", ex);
            }
        }

        public T GetOutput<T>(IJob forJob, string prop = null)
        {
            if (forJob == null) Ex.ArgNull(() => forJob);
            if (string.IsNullOrEmpty(prop)) Ex.ArgNull(() => prop);

            try
            {
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
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get output", ex);
            }
        }

        public void SetOutput<T>(IJob forJob, T value, string prop = null)
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


        public async Task<IReadOnlyList<IProcessJobValueField>>
            ProcessJobAsync(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            return await Task.FromResult(ProcessJob(definition, inputValues));
        }

        private sealed class _QueuedJob
        {
            public IJobDefinition definition;
            public IEnumerable<IProcessJobValueField> inputs;
            public Action<IReadOnlyList<IProcessJobValueField>, object> completion;
            public object uData;
        }

        private ConcurrentQueue<_QueuedJob> jobQueue = new ConcurrentQueue<_QueuedJob>();

        private Task JobRunnerTask;

        private CancellationTokenSource _jobCancellationSource;

        public void QueueJobAsync(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues, Action<IReadOnlyList<IProcessJobValueField>, object> onCompletion,
            object data)
        {
            lock (_lock)
            {
                if (JobRunnerTask == null)
                {
                    _jobCancellationSource = new CancellationTokenSource();
                    JobRunnerTask = Task.Factory.StartNew(DoJobRunner, _jobCancellationSource.Token);
                }

            }

            jobQueue.Enqueue(new _QueuedJob()
            {
                definition = definition,
                inputs = inputValues,
                completion = onCompletion,
                uData = data,
            });
        }

        private async void DoJobRunner()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (_jobCancellationSource.IsCancellationRequested)
                    {
                        // Do cancellation
                        break;
                    }
                }

                _QueuedJob qj = null;

                if (jobQueue.TryDequeue(out qj))
                {
                    // Got a job to do!

                    lock (_lock)
                    {
                        var res = ProcessJob(qj.definition, qj.inputs);
                        qj.completion(res, qj.uData);
                    }
                }
                else
                {
                    await Task.Delay(250);
                }
            }

            // Do cleanup
            JobRunnerTask = null;
            _jobCancellationSource = null;
        }


        public IEnumerable<IProcessJobValueField> QueueJobSynchronously(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            AutoResetEvent are = new AutoResetEvent(false);
            IEnumerable<IProcessJobValueField> res = null;
            QueueJobAsync(definition, inputValues, (r, d) =>
                {
                    res = r;
                    are.Set();
                },null);
            while (!are.WaitOne(150))
            {

            }

            return res;
        }
    }
}
