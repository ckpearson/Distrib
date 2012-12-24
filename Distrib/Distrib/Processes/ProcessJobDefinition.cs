using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public abstract class ProcessJobDefinition : IProcessJobDefinition
    {
        private readonly string _name;

        protected ProcessJobDefinition(
            string JobName)
        {
            _name = JobName;
        }

        string IProcessJobDefinition.Name
        {
            get { return _name; }
        }
    }

    public abstract class ProcessJobDefinition<T> : ProcessJobDefinition
    {
        public ProcessJobDefinition(string jobName)
            : base(jobName)
        {

        }
    }
}
