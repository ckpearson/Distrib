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
using Distrib.IOC;

namespace Distrib.Data.Transport
{
    public sealed class DataTransportPoint :
        IDataTransportPoint
    {
        private readonly string _name;
        private readonly DataTransportPointDirection _direction;
        private readonly IReadOnlyList<string> _keywords;
        private object _value;

        private readonly object _valLock = new object();

        public DataTransportPoint(
            [IOC(false)] string name,
            [IOC(false)] DataTransportPointDirection direction,
            [IOC(false)] IReadOnlyList<string> keywords,
            [IOC(false)] object value)
        {
            _name = name;
            _direction = direction;
            _keywords = keywords;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public DataTransportPointDirection Direction
        {
            get { return _direction; }
        }

        public IReadOnlyList<string> Keywords
        {
            get { return _keywords; }
        }

        public object Value
        {
            get
            {
                lock (_valLock)
                {
                    return _value;
                }
            }

            set
            {
                lock (_valLock)
                {
                    _value = value;
                }
            }
        }
    }
}
