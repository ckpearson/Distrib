﻿using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Modules
{
    [ModuleExport(typeof(ConnectionDetailsModule))]
    public sealed class ConnectionDetailsModule : IModule
    {
        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public ConnectionDetailsModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(AppRegions.ConnectionStatus, typeof(Views.ConnectionStatusView));
            _regionManager.RegisterViewWithRegion(AppRegions.ConnectionDetailsEdit, typeof(Views.ConnectionDetailsEditView));
            _regionManager.RegisterViewWithRegion(AppRegions.ConnectionDetailsInteract, typeof(Views.ConnectionDetailsInteractView));
        }
    }
}