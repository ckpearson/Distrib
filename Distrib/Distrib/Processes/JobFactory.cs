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

namespace Distrib.Processes
{
    public sealed class JobFactory : CrossAppDomainObject, IJobFactory
    {
        private readonly IIOC _ioc;
        
        public JobFactory([IOC(true)] IIOC ioc)
        {
            _ioc = ioc;
        }

        public IJob CreateJob(IJobInputTracker inputTracker, IJobOutputTracker outputTracker, IJobDefinition jobDefinition)
        {
            return _ioc.Get<IJob>(new []
            {
                new IOCConstructorArgument("inputTracker", inputTracker),
                new IOCConstructorArgument("outputTracker", outputTracker),
                new IOCConstructorArgument("jobDefinition", jobDefinition),
            });
        }
    }
}
