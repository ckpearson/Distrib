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
