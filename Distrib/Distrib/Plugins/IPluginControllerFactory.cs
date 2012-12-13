﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginControllerFactory
    {
        IPluginController CreateController();
        IPluginController CreateControllerOfType(Type type, IPluginControllerValidationService controllerValidator);
    }
}
