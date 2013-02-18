﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.IOC;

namespace Distrib.Processes
{
    public sealed class TypePoweredProcessHost
       : ProcessHostBase,
       ITypePoweredProcessHost
    {
        private readonly Type _instanceType;

        private DateTime _creationStamp;
        private string _creationId;

        public TypePoweredProcessHost([IOC(false)] Type instanceType,
            [IOC(true)] IJobFactory jobFactory)
            : base(jobFactory)
        {
            _instanceType = instanceType;
        }

        protected override void DoInit()
        {
            try
            {
                lock (_lock)
                {
                    _processInstance = (IProcess)Activator.CreateInstance(_instanceType);
                    _creationStamp = DateTime.Now;
                    _creationId = Guid.NewGuid().ToString();
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

                    _processInstance = null;
                    _creationStamp = DateTime.MinValue;
                    _creationId = null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to perform uninit", ex);
            }
        }

        protected override DateTime GetInstanceCreationStamp()
        {
            return _creationStamp;
        }

        protected override string GetInstanceID()
        {
            return _creationId;
        }

        public Type InstanceType
        {
            get { return _instanceType; }
        }
    }
}