using Microsoft.Practices.Prism.Events;
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

namespace ProcessNode.Events
{
    public interface INewEventAggregator
    {
        void Send<T>(T message);
        void Subscribe<T>(Action<T> action);
        void Subscribe<T>(Action<T> action, bool keepAlive);
        void Subscribe<T>(Action<T> action, ThreadOption threadOption);
        void Subscribe<T>(Action<T> action, ThreadOption threadOption, bool keepAlive);
        void Unsubscribe<T>(Action<T> action);
    }
}
