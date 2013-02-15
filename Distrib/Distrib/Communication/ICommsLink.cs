using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents a comms link
    /// </summary>
    public interface ICommsLink
    {
        CommsDirection PrimaryDirection { get; }
    }
}
