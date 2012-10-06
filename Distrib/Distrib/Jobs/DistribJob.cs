using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Jobs
{
    /// <summary>
    /// Class representing a job for a process to perform
    /// </summary>
    public sealed class DistribJob
    {

    }
}

namespace Distrib
{
    public static class Utils
    {
        public static bool ParsesAsGuid(this string str)
        {
            Guid guid = Guid.Empty;

            return Guid.TryParse(str, out guid);
        }
    }
}