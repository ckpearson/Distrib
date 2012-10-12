using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    public static class DistribPluginTypes
    {
        /// <summary>
        /// Plugin type for a processing node process
        /// </summary>
        public static readonly DistribPluginType DistribProcess =
            DistribPluginType.Create<IDistribProcess>("DistribProcess",
            "Represents a process for a distrib processing node");

        /// <summary>
        /// Gets a list of the different plugin types supported
        /// </summary>
        /// <returns>The list of <see cref="DistribPluginType"/></returns>
        public static List<DistribPluginType> GetPluginTypes()
        {
            List<DistribPluginType> types = new List<DistribPluginType>();

            try
            {
                return typeof(DistribPluginTypes).GetFields()
                    .Where(f => f.FieldType == typeof(DistribPluginType)).Select(f => f.GetValue(null) as DistribPluginType).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get plugin types", ex);
            }
        }
    }
}
