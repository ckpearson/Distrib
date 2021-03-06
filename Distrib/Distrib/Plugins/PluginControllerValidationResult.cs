﻿/*
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
    public enum PluginControllerValidationResult
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
    }
}
