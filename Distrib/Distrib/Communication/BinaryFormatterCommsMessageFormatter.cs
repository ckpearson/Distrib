using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Serialises and deserialises comms messages using a binary formatter
    /// </summary>
    public sealed class BinaryFormatterCommsMessageFormatter : ICommsMessageFormatter
    {
        public byte[] Serialise(ICommsMessage message)
        {
            if (message == null) throw Ex.ArgNull(() => message);

            try
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
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to serialise comms message", ex);
            }
        }

        public ICommsMessage Deserialise(byte[] data)
        {
            if (data == null || data.Length == 0) throw Ex.ArgNull(() => data);

            try
            {
                var bf = new BinaryFormatter();
                ICommsMessage msg = null;
                using (var ms = new MemoryStream(data))
                {
                    ms.Position = 0;
                    msg = (ICommsMessage)bf.Deserialize(ms);
                }
                return msg;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to deserialise comms message", ex);
            }
        }
    }
}
