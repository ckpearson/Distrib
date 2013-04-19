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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Distrib.Storage
{
    /// <summary>
    /// Represents a persistence record collection
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("pr")]
    public sealed class PersistRecords :
        System.Xml.Serialization.IXmlSerializable
    {
        private Dictionary<string, object> _dict = new Dictionary<string, object>();
        private readonly object _lock = new object();

        private bool _readonly = false;

        private string _name;

        private Type _iPersistParentType;

        public PersistRecords()
            : this(Guid.NewGuid().ToString(), false, new Dictionary<string, object>())
        {

        }

        public PersistRecords(string name)
            : this(name, false, new Dictionary<string, object>())
        {

        }

        private PersistRecords(string name, bool readOnly, Dictionary<string, object> initCollection)
        {
            _name = name;
            _readonly = readOnly;
            _dict = initCollection;
        }

        /// <summary>
        /// Gets or sets the record of the given name
        /// </summary>
        /// <param name="record">The name of the record</param>
        /// <returns>The record item</returns>
        public object this[string record]
        {
            get
            {
                lock (_lock)
                {
                    var item = _dict[record];

                    /*
                     * If the item is a persist records collection then it's because
                     * the item was an IPersist-aware type and now it needs
                     * creating and loading with the values
                     */
                    if (item is PersistRecords)
                    {
                        var pr = (PersistRecords)item;
                        IPersist inst;

                        try
                        {
                            inst = (IPersist)Activator.CreateInstance(pr._iPersistParentType);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(
                                string.Format("Failed to create instance of type '{0}' to load persisted data",
                                pr._iPersistParentType), ex);
                        }

                        try
                        {
                            inst.LoadFromPersisted(pr);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Failed to get the instance to load from persisted records", ex);
                        }

                        return inst;
                    }
                    else
                    {
                        return item;
                    }
                }
            }

            set
            {
                if (_readonly)
                {
                    throw new InvalidOperationException("Readonly");
                }

                lock (_lock)
                {
                    if (value != null)
                    {
                        if (!Attribute.IsDefined(value.GetType(), typeof(SerializableAttribute)) &&
                            value.GetType().GetInterface(typeof(IPersist).FullName) == null)
                        {
                            throw new ArgumentException("Value must either be serializable or support 'IPersist'", "value");
                        }

                        /*
                         * If the value is an IPerist-aware type build up a new records
                         * and get the instance to persist to it, store the records instead
                         * of the instance
                         */
                        if (value.GetType().GetInterface(typeof(IPersist).FullName) != null)
                        {
                            try
                            {
                                var pr = new PersistRecords(Guid.NewGuid().ToString(), false, new Dictionary<string, object>());
                                pr._iPersistParentType = value.GetType();
                                ((IPersist)value).PersistRecords(pr);
                                _dict[record] = pr;
                                return;
                            }
                            catch (Exception ex)
                            {
                                throw new ApplicationException(
                                    string.Format("Failed to turn record item of type '{0}' into persist records " +
                                    "collection as it is IPersist-aware", value.GetType()), ex);
                            }
                        }

                        _dict[record] = value;
                    }
                    else
                    {
                        _dict[record] = null;
                    }
                }
            }
        }

        System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            var xdoc = XDocument.Parse(reader.ReadOuterXml());

            _name = xdoc.Root.Attribute("n").Value;
            _readonly = Convert.ToBoolean(xdoc.Root.Attribute("o").Value);

            var it = xdoc.Root.Attribute("pt");

            if (it != null)
            {
                _iPersistParentType = Type.GetType(it.Value);
            }
            foreach (var item in xdoc.Root.Element("d").Elements("i"))
            {
                var typ = Type.GetType(item.Attribute("t").Value);
                var key = item.Attribute("k").Value;

                var xser = new System.Xml.Serialization.XmlSerializer(typ);
                var val = xser.Deserialize(item.FirstNode.CreateReader());
                _dict.Add(key, val);
            }
        }

        void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("n", _name);
            writer.WriteAttributeString("o", _readonly.ToString());
            if (_iPersistParentType != null)
            {
                writer.WriteAttributeString("pt", _iPersistParentType.AssemblyQualifiedName);
            }
            writer.WriteStartElement("d");
            lock (_dict)
            {
                foreach (var item in _dict)
                {
                    writer.WriteStartElement("i");
                    writer.WriteAttributeString("k", item.Key);
                    writer.WriteAttributeString("t", item.Value.GetType().AssemblyQualifiedName);

                    var xser = new System.Xml.Serialization.XmlSerializer(item.Value != null ? item.Value.GetType() : typeof(object));
                    xser.Serialize(writer, item.Value);

                    writer.WriteEndElement();

                }
            }

            writer.WriteEndElement();
        }
    }
}
