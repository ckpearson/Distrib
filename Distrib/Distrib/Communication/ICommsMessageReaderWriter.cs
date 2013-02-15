using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Distrib.Communication
{
    public interface ICommsMessageReaderWriter
    {
        string Write(ICommsMessage message);
        ICommsMessage Read(string data);
    }

    public sealed class XDocumentCommsMessageReaderWriter : ICommsMessageReaderWriter
    {
        private readonly ICommsMessageSerialiserDeserializer _serDeser;

        public XDocumentCommsMessageReaderWriter(ICommsMessageSerialiserDeserializer serDeser)
        {
            _serDeser = serDeser;
        }

        public string Write(ICommsMessage message)
        {
            var dat = new XDocument(
                new XElement("cmsg",
                    Convert.ToBase64String(_serDeser.Serialise(message))))
                    .ToString(SaveOptions.DisableFormatting);

            return dat;
        }

        public ICommsMessage Read(string data)
        {
            var xdoc = XDocument.Parse(data, LoadOptions.None);
            var msg = _serDeser.Deserialise(
                Convert.FromBase64String(
                    xdoc.Element("cmsg").Value));
            return msg;
        }
    }

    public interface ICommsMessageSerialiserDeserializer
    {
        byte[] Serialise(ICommsMessage message);
        ICommsMessage Deserialise(byte[] data);
    }

    public sealed class BinaryFormatterCommsMessageSerialiserDeserialiser: ICommsMessageSerialiserDeserializer
    {
        public byte[] Serialise(ICommsMessage message)
        {
            var bf = new BinaryFormatter();
            var data = new byte[] { };
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, message);
                ms.Flush();
                ms.Position = 0;
                data = ms.ToArray();
            }

            return data;
        }

        public ICommsMessage Deserialise(byte[] data)
        {
            var bf = new BinaryFormatter();
            ICommsMessage message = null;
            using (var ms = new MemoryStream(data))
            {
                ms.Position = 0;
                message = (ICommsMessage)bf.Deserialize(ms);
            }
            return message;
        }
    }
}
