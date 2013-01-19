using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.IOC;
using System.Linq.Expressions;
using Distrib.Utils;
using System.Reflection;

namespace Distrib.Processes
{
    public abstract class ProcessJobDefinitionBase : CrossAppDomainObject, IJobDefinition
    {
        private readonly Type _inputInterfaceType;
        private readonly Type _outputInterfaceType;
        private readonly string _jobName;

        private readonly LockValue<List<IProcessJobDefinitionField>> _fields =
            new LockValue<List<IProcessJobDefinitionField>>(new List<IProcessJobDefinitionField>());

        public ProcessJobDefinitionBase(
            [IOC(false)] string jobName,
            [IOC(false)] Type inputInterfaceType,
            [IOC(false)] Type outputInterfaceType)
        {
            if (string.IsNullOrEmpty(jobName)) throw Ex.ArgNull(() => jobName);
            if (inputInterfaceType == null) throw Ex.ArgNull(() => inputInterfaceType);
            if (outputInterfaceType == null) throw Ex.ArgNull(() => outputInterfaceType);

            if (!inputInterfaceType.IsInterface) throw Ex.Arg(() => inputInterfaceType,
                "Must be an interface");

            if (!outputInterfaceType.IsInterface) throw Ex.Arg(() => outputInterfaceType,
                "Must be an interface");

            _jobName = jobName;
            _inputInterfaceType = inputInterfaceType;
            _outputInterfaceType = outputInterfaceType;

            _buildInitialFields();
        }

        private void _buildInitialFields()
        {
            try
            {
                _fields.ReadWrite((fieldsList) =>
                    {

                        var foundFields =
                            (_inputInterfaceType.GetProperties()
                                .Where(p => p.CanRead && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable))
                                .Select(p => ProcessJobFieldFactory.CreateDefinitionField(p.PropertyType, p.Name, FieldMode.Input))
                            .Concat(
                                _outputInterfaceType.GetProperties()
                                .Where(p => (p.CanRead && p.CanWrite) && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable)
                                .Select(p => ProcessJobFieldFactory.CreateDefinitionField(p.PropertyType, p.Name, FieldMode.Output))).ToList();

                        if (foundFields == null || foundFields.Count == 0)
                        {
                            throw new ApplicationException("No fields could be found on either the input or output types");
                        }

                        return new List<IProcessJobDefinitionField>(foundFields);
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to build initial fields", ex);
            }
        }

        public IReadOnlyList<IProcessJobDefinitionField> InputFields
        {
            get
            {
                return _fields.Value.Where(f => f.Mode == FieldMode.Input)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public IReadOnlyList<IProcessJobDefinitionField> OutputFields
        {
            get
            {
                return _fields.Value.Where(f => f.Mode == FieldMode.Output)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public string Name
        {
            get { return _jobName; }
        }

        protected IProcessJobDefinitionField GetField(PropertyInfo pi)
        {
            if (pi == null) throw Ex.ArgNull(() => pi);

            try
            {
                return _fields.Value.SingleOrDefault(f => f.Name == pi.Name && f.Type.Equals(pi.PropertyType));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get field", ex);
            }
        }

        protected void ReplaceField(IProcessJobDefinitionField field, IProcessJobDefinitionField replacement)
        {
            if (field == null) throw Ex.ArgNull(() => field);
            if (replacement == null) throw Ex.ArgNull(() => replacement);

            try
            {
                _fields.ReadWrite((fields) =>
                    {
                        var index = fields.IndexOf(field);
                        if (index < 0)
                        {
                            throw new InvalidOperationException("Couldn't find field in fields list");
                        }

                        fields[index] = replacement;

                        return fields;
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to replace field", ex);
            }
        }
    }
}
