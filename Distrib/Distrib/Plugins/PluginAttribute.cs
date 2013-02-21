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
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class PluginAttribute : Attribute
    {
        private readonly string _name = "";
        private readonly string _description = "";
        private readonly double _version = 0.0;
        private readonly string _author = "";
        private readonly string _identifier = "";

        private readonly Type _interfaceType;

        private readonly WriteOnce<Type> _controllerType = new WriteOnce<Type>(null);

        private readonly WriteOnce<IReadOnlyList<PluginAdditionalMetadataObject>> _lstRawSuppliedMetadataObjects =
            new WriteOnce<IReadOnlyList<PluginAdditionalMetadataObject>>();

        public PluginAttribute(Type interfaceType,
            string name,
            string description,
            double version,
            string author,
            string identifier,
            Type controllerType = null)
        {
            _interfaceType = interfaceType;
            _name = name;
            _description = description;
            _version = version;
            _author = author;
            _identifier = identifier;
            _controllerType.Value = controllerType;
        }

        public Type InterfaceType
        {
            get { return _interfaceType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public double Version
        {
            get { return _version; }
        }

        public string Author
        {
            get { return _author; }
        }

        public string Identifier
        {
            get { return _identifier; }
        }

        public Type ControllerType
        {
            get
            {
                lock (_controllerType)
                {
                    return (!_controllerType.IsWritten) ? null : _controllerType.Value;
                }
            }

            set
            {
                lock (_controllerType)
                {
                    if (!_controllerType.IsWritten)
                    {
                        _controllerType.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Controller type already specified");
                    }
                }
            }
        }

        public IReadOnlyList<PluginAdditionalMetadataObject> SuppliedMetadataObjects
        {
            get
            {
                lock (_lstRawSuppliedMetadataObjects)
                {
                    return (!_lstRawSuppliedMetadataObjects.IsWritten) ? null : _lstRawSuppliedMetadataObjects.Value;
                }
            }
            
            protected set
            {
                lock (_lstRawSuppliedMetadataObjects)
                {
                    if (!_lstRawSuppliedMetadataObjects.IsWritten)
                    {
                        _lstRawSuppliedMetadataObjects.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Supplied metadata objects already set");
                    }
                }
            }
        }
    }
}
