using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Distrib.Processes
{
    /// <summary>
    /// Core job descriptor
    /// </summary>
    [Serializable()]
    public sealed class JobDescriptor : IJobDescriptor
    {
        private readonly string _name;
        private readonly IReadOnlyList<IProcessJobDefinitionField> _inputFields;
        private readonly IReadOnlyList<IProcessJobDefinitionField> _outputFields;

        private JobDescriptor() { }

        public JobDescriptor(string jobName,
            IReadOnlyList<IProcessJobDefinitionField> inputFields,
            IReadOnlyList<IProcessJobDefinitionField> outputFields)
        {
            _name = jobName;
            _inputFields = inputFields;
            _outputFields = outputFields;
        }

        public string JobName
        {
            get { return _name; }
        }

        public IReadOnlyList<IProcessJobDefinitionField> InputFields
        {
            get { return _inputFields; }
        }

        public IReadOnlyList<IProcessJobDefinitionField> OutputFields
        {
            get { return _outputFields; }
        }
    }
}
