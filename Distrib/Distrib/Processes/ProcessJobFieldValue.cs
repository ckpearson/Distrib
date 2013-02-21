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

namespace Distrib.Processes
{
    [Serializable()]
    internal class ProcessJobFieldValue : IProcessJobValueField
    {
        private readonly IProcessJobDefinitionField _definition;
        private object _value;
        private readonly object _lock = new object();

        public ProcessJobFieldValue(IProcessJobDefinitionField definition)
        {
            _definition = definition;
        }

        public IProcessJobDefinitionField Definition
        {
            get { return _definition; }
        }

        public object Value
        {
            get
            {
                lock (_lock)
                {
                    return _value;
                }
            }

            set
            {
                lock (_lock)
                {
                    _value = value;
                }
            }
        }
    }

    [Serializable()]
    internal sealed class ProcessJobFieldValue<T> : ProcessJobFieldValue, IProcessJobValueField<T>
    {
        public ProcessJobFieldValue(IProcessJobDefinitionField<T> definition)
            : base(definition)
        {

        }

        public new IProcessJobDefinitionField<T> Definition
        {
            get { return (IProcessJobDefinitionField<T>)base.Definition; }
        }

        public new T Value
        {
            get
            {
                return (T)base.Value;
            }
            set
            {
                base.Value = value;
            }
        }
    }
}
