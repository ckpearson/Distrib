using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DistribApps.Core.Processes.Hosting
{
    public interface IHostProvider
    {
        string Name { get; }
        string Description { get; }
        string UIFromText { get; }

        bool HasUI { get; }

        UserControl CreateWithUI(out Func<UserControl, string> validationFunc,
            out Func<UserControl, IProcessHost> creationAction);

        IProcessHost CreateWithoutUI();
    }
}
