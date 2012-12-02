using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery.Metadata
{
    public interface IPluginMetadata
    {
        Type InterfaceType { get; }
        Type ControllerType { get; set; }
        string Name { get; }
        string Description { get; }
        double Version { get; }
        string Author { get; }
        string Identifier { get; }
    }
}
