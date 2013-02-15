/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/

using Distrib.Communication;
using Distrib.IOC;
using Distrib.IOC.Ninject;
using Distrib.Nodes.Process;
using Distrib.Nodes.Process.HostSources;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Stock;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
    class Program
    {
        private string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "distrib plugins");

        static void Main(string[] args)
        {
            var p = new Program();

            int port = 8080;
            var tcpIncomingLink = new TcpIncomingCommsLink<IAbcComms>(IPAddress.Any,
                port,
                new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()),
                new DirectInvocationCommsMessageProcessor());

            var abc = new Abc(tcpIncomingLink);

            var tcpOut = new TcpOutgoingCommsLink<IAbcComms>(IPAddress.Loopback, port,
                new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()));

            var abcProx = new AbcOutgoingProxy(tcpOut);
            //string sHello = abcProx.SayHello("Clint");

            int num = abcProx.Number;

            string s = abcProx.SayHello("Clint");

            abcProx.Number = 150;

            num = abcProx.Number;

            Console.ReadLine();
        }
    }

    public interface IAbcComms
    {
        void DoSomethingOverComms();
        string SayHello(string name);

        int Number { get; set; }
    }

    public interface IAbc
    {
        void DoSomething();
    }

    public sealed class Abc : IAbc, IAbcComms, IDisposable
    {
        private IIncomingCommsLink<IAbcComms> _incomingComms;

        public Abc(IIncomingCommsLink<IAbcComms> incomingComms)
        {
            _incomingComms = incomingComms;
            _incomingComms.StartListening(this as IAbcComms);
        }

        public void DoSomething()
        {
            
        }

        public void Dispose()
        {
            if (_incomingComms != null)
            {
                if (_incomingComms.IsListening)
                {
                    _incomingComms.StopListening();
                }
            }
        }

        public void DoSomethingOverComms()
        {
        }

        public string SayHello(string name)
        {
            return string.Format("Hello, {0}", name);
        }

        public int Number
        {
            get;
            set;
        }
    }

    public sealed class AbcOutgoingProxy : OutgoingCommsProxyBase, IAbcComms
    {
        public AbcOutgoingProxy(IOutgoingCommsLink<IAbcComms> link)
            : base(link)
        {

        }

        public void DoSomethingOverComms()
        {
            this.Link.InvokeMethod(null);
        }


        public string SayHello(string name)
        {
            return (string)this.Link.InvokeMethod(new[] { name });
        }
        public int Number
        {
            get { return (int)this.Link.GetProperty(); }
            set
            {
                this.Link.SetProperty(value);
            }
        }
    }
}