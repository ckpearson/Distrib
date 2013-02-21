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
using System.Xml.Linq;

namespace Distrib.Communication
{
    /// <summary>
    /// Comms message reader and writer that uses XML
    /// </summary>
    public sealed class XmlCommsMessageReaderWriter : ICommsMessageReaderWriter
    {
        private readonly ICommsMessageFormatter _serDeser;

        private const string Element_Name = "cmsg";

        public XmlCommsMessageReaderWriter(ICommsMessageFormatter serDeser)
        {
            if (serDeser == null) throw Ex.ArgNull(() => serDeser);

            _serDeser = serDeser;
        }

        public string Write(ICommsMessage message)
        {
            if (message == null) throw Ex.ArgNull(() => message);

            try
            {
                return new XDocument(
                    new XElement(Element_Name,
                        Convert.ToBase64String(
                            _serDeser.Serialise(
                                message)))).ToString(SaveOptions.DisableFormatting);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to write comms message to XML", ex);
            }
        }

        public ICommsMessage Read(string data)
        {
            if (string.IsNullOrEmpty(data)) throw Ex.ArgNull(() => data);

            try
            {
                return _serDeser
                    .Deserialise(
                        Convert.FromBase64String(
                            XDocument.Parse(data)
                                .Element(Element_Name)
                                .Value));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to read comms message from XML", ex);
            }
        }
    }
}
