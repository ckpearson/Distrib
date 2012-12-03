using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old.Controllers
{
    /// <summary>
    /// The result of validating a controller type
    /// </summary>
    internal enum DistribPluginControllerValidationResult
    {
        /// <summary>
        /// The controller type checked out
        /// </summary>
        Success = 0,

        /// <summary>
        /// There was an unforseen problem / reason for validation failure
        /// </summary>
        UnknownFailure,

        /// <summary>
        /// The controller type isn't marshalable
        /// </summary>
        ControllerTypeNotMarshalable,

        /// <summary>
        /// The controller type isn't a class
        /// </summary>
        ControllerTypeNotAClass,

        /// <summary>
        /// The controller type doesn't implement the controller interface
        /// </summary>
        ControllerInterfaceNotImplemented,

        /// <summary>
        /// The controller type doesn't have a public parameterless constructor
        /// </summary>
        ControllerTypeMissingPublicParameterlessConstructor,
    }
}
