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
