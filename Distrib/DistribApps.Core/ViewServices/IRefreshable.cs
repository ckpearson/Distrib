using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Core.ViewServices
{
    public interface IRefreshable
    {
        bool CanRefresh { get; }
        void Refresh();
    }
}
