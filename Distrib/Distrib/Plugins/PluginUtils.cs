using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public static class PluginUtils
    {
        public static bool OfPluginInterface<T>(this IPluginDescriptor descriptor)
            where T : class
        {
            if (descriptor == null) throw new ArgumentNullException();

            try
            {
                if (!typeof(T).IsInterface) throw new InvalidOperationException("T must be an interface");

                return descriptor.Metadata.InterfaceType.Equals(typeof(T));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to check if descriptor is for given plugin interface", ex);
            }
        }
    }
}
