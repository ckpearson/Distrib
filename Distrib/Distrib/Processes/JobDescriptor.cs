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
        private readonly IReadOnlyList<IProcessJobField> _inputFields;
        private readonly IReadOnlyList<IProcessJobField> _outputFields;

        private JobDescriptor() { }

        public JobDescriptor(string jobName,
            IReadOnlyList<IProcessJobField> inputFields,
            IReadOnlyList<IProcessJobField> outputFields)
        {
            _name = jobName;
            _inputFields = inputFields;
            _outputFields = outputFields;
        }

        public string JobName
        {
            get { return _name; }
        }

        public IReadOnlyList<IProcessJobField> InputFields
        {
            get { return _inputFields; }
        }

        public IReadOnlyList<IProcessJobField> OutputFields
        {
            get { return _outputFields; }
        }
    }
}
