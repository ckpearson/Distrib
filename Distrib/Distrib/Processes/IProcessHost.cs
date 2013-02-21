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
using Distrib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessHost
    {
        void Initialise();
        void Unitialise();

        bool IsInitialised { get; }

        IReadOnlyList<IJobDefinition> JobDefinitions { get; }

        void QueueJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues,
            Action<IReadOnlyList<IProcessJobValueField>, object> onCompletion, object data,
            Action<Exception> onException);

        IReadOnlyList<IProcessJobValueField> QueueJobAndWait(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues);

        int QueuedJobs { get; }

        bool IsJobExecuting { get; }

        IJobDefinition CurrentlyExecutingJob { get; }
  

        DateTime InstanceCreationStamp { get; }
        string InstanceID { get; }

        IProcessMetadata Metadata { get; }
    }

    public interface IPluginPoweredProcessHost : IProcessHost
    {
        IPluginDescriptor PluginDescriptor { get; }
        IReadOnlyList<string> DeclaredDependentAssemblies { get; }
    }

    public interface ITypePoweredProcessHost : IProcessHost
    {
        //void QueueJobAsync(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues, Action<IReadOnlyList<IProcessJobValueField>, object> onCompletion, object data);
        //IEnumerable<IProcessJobValueField> QueueJobSynchronously(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues);
        Type InstanceType { get; }
    }
}
