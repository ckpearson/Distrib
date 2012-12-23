using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public abstract class ProcessJobBase : IProcessJob
    {
        private readonly string _ident;
        private readonly DateTime _creationStamp;

        protected ProcessJobBase()
        {
            _ident = Guid.NewGuid().ToString();
            _creationStamp = DateTime.Now;
        }

        string IProcessJob.Identifier
        {
            get { return _ident; }
        }

        DateTime IProcessJob.CreationTimeStamp
        {
            get { return _creationStamp; }
        }

        ProcessJobStatus IProcessJob.Status
        {
            get { throw new NotImplementedException(); }
        }
    }
}
