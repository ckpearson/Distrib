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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.IOC;
using Distrib.Processes.TypePowered;
using System.Reflection;

namespace Distrib.Processes
{
    public sealed class TypePoweredProcessHost
       : ProcessHostBase,
       ITypePoweredProcessHost
    {
        private readonly Type _instanceType;

        private DateTime _creationStamp;
        private string _creationId;

        private IProcessMetadata _metadata;

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
                    if (!Attribute.IsDefined(_instanceType, typeof(ProcessMetadataAttribute)))
                    {
                        throw new InvalidOperationException("Process type isn't decorated with the metadata attribute");
                    }

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

        protected override IProcessMetadata GetMetadataObject()
        {
            if (Attribute.IsDefined(_instanceType, typeof(ProcessMetadataAttribute)))
            {
                var attr = _instanceType.GetCustomAttribute<ProcessMetadataAttribute>();
                return (IProcessMetadata)attr.MetadataObject;
            }
            else
            {
                return null;
            }
        }
    }
}
