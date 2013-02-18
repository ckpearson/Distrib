using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib;
using Distrib.IOC;
using Distrib.Processes;
using System.Collections.Concurrent;
using System.Threading;

namespace Distrib.Processes
{
    public abstract class ProcessHostBase :
        CrossAppDomainObject,
        IProcessHost,
        IJobInputTracker,
        IJobOutputTracker,
        IDisposable
    {
        private readonly IJobFactory _jobFactory;

        protected readonly object _lock = new object();
        protected IProcess _processInstance;

        private bool _isInitialised;

        public ProcessHostBase(
            [IOC(true)] IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
        }

        protected abstract void DoInit();

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

                    try
                    {
                        DoInit();

                        _isInitialised = true;
                    }
                    catch (Exception ex)
                    {
                        _isInitialised = false;

                        throw new ApplicationException("Process host failed to perform initialisation", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise process host", ex);
            }
        }

        protected abstract void DoUninit();

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

                    try
                    {
                        DoUninit();
                        if (_jobRunnerTaskCancellationSource != null && !_jobRunnerTaskCancellationSource.IsCancellationRequested)
                        {
                            _jobRunnerTaskCancellationSource.Cancel();
                        }
                        _isInitialised = false;
                    }
                    catch (Exception ex)
                    {
                        _isInitialised = false;
                        throw new ApplicationException("Process host failed to perform uninitialisation", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to uninitialise process host", ex);
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

        private class _QueuedJob
        {
            public IJobDefinition definition;
            public IEnumerable<IProcessJobValueField> inputs;
            public Action<IReadOnlyList<IProcessJobValueField>, object> completed;
            public Action<Exception> onException;
            public object data;
        }

        private Task _jobRunnerTask;
        private CancellationTokenSource _jobRunnerTaskCancellationSource;

        private ConcurrentQueue<_QueuedJob> _jobQueue = new ConcurrentQueue<_QueuedJob>();

        private bool _isJobExecuting = false;

        private IJobDefinition _executingJob;

        private readonly object _jobTaskLock = new object();

        public void QueueJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues,
            Action<IReadOnlyList<IProcessJobValueField>, object> onCompletion, object data,
            Action<Exception> onException)
        {
            lock (_jobTaskLock)
            {
                if (_jobRunnerTask == null)
                {
                    _jobRunnerTaskCancellationSource = new CancellationTokenSource();
                    _jobRunnerTask = Task.Factory.StartNew(_DoJobRunner, _jobRunnerTaskCancellationSource.Token);
                }
            }

            _jobQueue.Enqueue(new _QueuedJob()
            {
                definition = definition,
                inputs = inputValues,
                data = data,
                completed = onCompletion,
                onException = onException,
            });
        }


        public IReadOnlyList<IProcessJobValueField> QueueJobAndWait(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            var are = new AutoResetEvent(false);
            IReadOnlyList<IProcessJobValueField> res = null;
            Exception except = null;
            QueueJob(definition, inputValues, (r, d) =>
            {
                res = r;
                are.Set();
            }, null, (ex) =>
            {
                except = ex;
                are.Set();
            });

            while (!are.WaitOne(100))
            {

            }

            if (except != null)
            {
                throw new ApplicationException("An error occurred", except);
            }
            else
            {
                return res;
            }
        }


        public int QueuedJobs
        {
            get { return _jobQueue.Count; }
        }

        public bool JobExecuting
        {
            get
            {
                lock (_jobTaskLock)
                {
                    return _isJobExecuting;
                }
            }
        }

        public IJobDefinition ExecutingJob
        {
            get
            {
                lock (_jobTaskLock)
                {
                    return _executingJob;
                }
            }
        }

        private async void _DoJobRunner()
        {
            while (true)
            {
                lock (_jobTaskLock)
                {
                    if (_jobRunnerTaskCancellationSource.IsCancellationRequested)
                    {
                        // Do some cancel cleanup

                        break;
                    }

                }

                _QueuedJob qj = null;

                if (_jobQueue.TryDequeue(out qj))
                {
                    // Process job

                    lock (_jobTaskLock)
                    {
                        _isJobExecuting = true;
                        _executingJob = qj.definition;
                    }

                    IReadOnlyList<IProcessJobValueField> res = null;

                    try
                    {
                        res = _processJob(qj.definition, qj.inputs);
                    }
                    catch (Exception ex)
                    {
                        if (qj.onException != null)
                        {
                            try
                            {
                                qj.onException(ex);
                            }
                            catch { }
                        }
                    }
                    finally
                    {
                        lock (_jobTaskLock)
                        {
                            _isJobExecuting = false;
                            _executingJob = null;
                        }
                    }


                    if (qj.completed != null)
                    {
                        try
                        {
                            qj.completed(res, qj.data);
                        }
                        catch { }
                    }
                }
                else
                {
                    // Wait to check again
                    await Task.Delay(150);
                }
            }

            lock (_jobTaskLock)
            {
                _jobRunnerTaskCancellationSource = null;
                _jobRunnerTask = null;
            }
        }

        private IReadOnlyList<IProcessJobValueField> _processJob(IJobDefinition jobDefinition, IEnumerable<IProcessJobValueField> inputValues)
        {
            if (!IsInitialised)
            {
                throw new InvalidOperationException("Can't process job, not initialised");
            }

            try
            {
                IJob job = _jobFactory.CreateJob(this, this, jobDefinition);
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
                    lock (_processInstance)
                    {
                        _processInstance.ProcessJob(job);
                    }
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

        protected abstract DateTime GetInstanceCreationStamp();

        public DateTime InstanceCreationStamp
        {
            get { return GetInstanceCreationStamp(); }
        }

        protected abstract string GetInstanceID();

        public string InstanceID
        {
            get { return GetInstanceID(); }
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


        private bool _disposed = false;
        ~ProcessHostBase()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                return;
            }
            else
            {

                try
                {
                    lock (_jobTaskLock)
                    {
                        if (_jobRunnerTaskCancellationSource != null && !_jobRunnerTaskCancellationSource.IsCancellationRequested)
                        {
                            _jobRunnerTaskCancellationSource.Cancel();
                        }
                    }
                }
                catch { }

                // Dispose crossAppDomain
                base.Dispose(disposing);
                _disposed = true;
            }
        }

    }
}
