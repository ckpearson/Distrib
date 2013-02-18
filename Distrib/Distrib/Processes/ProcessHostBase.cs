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
        IJobOutputTracker
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

        

        public IReadOnlyList<IProcessJobValueField> ProcessJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IProcessJobValueField>> ProcessJobAsync(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            throw new NotImplementedException();
        }

        public DateTime InstanceCreationStamp
        {
            get { throw new NotImplementedException(); }
        }

        public string InstanceID
        {
            get { throw new NotImplementedException(); }
        }

        public T GetInput<T>(IJob forJob, string prop = null)
        {
            throw new NotImplementedException();
        }

        public T GetOutput<T>(IJob forJob, string prop = null)
        {
            throw new NotImplementedException();
        }

        public void SetOutput<T>(IJob forJob, T value, string prop = null)
        {
            throw new NotImplementedException();
        }
    }
}
