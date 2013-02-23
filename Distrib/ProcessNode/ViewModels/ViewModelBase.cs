using Microsoft.Practices.Prism;
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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.ViewModels
{
    public abstract class ViewModelBase :
        INotifyPropertyChanged,
        INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool IsNavTarget(NavigationContext context)
        {
            return true;
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return IsNavTarget(navigationContext);
        }

        protected virtual void OnNavigatedAway(NavigationContext context) { }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            OnNavigatedAway(navigationContext);
        }

        protected virtual void OnNavigatedTo(NavigationContext context) { }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            OnNavigatedTo(navigationContext);
        }

        protected void PropChange([CallerMemberName] string property = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
