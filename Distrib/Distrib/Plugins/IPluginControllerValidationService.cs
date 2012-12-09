using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginControllerValidationService
    {
        Res<Type, PluginControllerValidationResult> ValidateControllerType(Type controllerType);
    }
}
