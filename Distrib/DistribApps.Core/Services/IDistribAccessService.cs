using Distrib.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Core.Services
{
    public interface IDistribAccessService
    {
        IIOC DistribIOC { get; }
    }
}
