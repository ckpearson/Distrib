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
