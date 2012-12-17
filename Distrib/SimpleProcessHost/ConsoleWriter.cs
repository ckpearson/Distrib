using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProcessHost
{
    public sealed class ConsoleWriter : TextWriter
    {
        private readonly TextWriter _oldWriter;
        private bool _doIndent = false;

        public ConsoleWriter()
        {
            _oldWriter = Console.Out;
            Console.SetOut(this);
        }

        public int Indent { get; set; }

        public override void Write(char value)
        {
            if (_doIndent)
            {
                _doIndent = false;
                for (int x = 0; x < Indent; ++x) _oldWriter.Write("  ");
            }
            _oldWriter.Write(value);
            if (value == '\n') _doIndent = true;
        }

        public override Encoding Encoding
        {
            get { return _oldWriter.Encoding; }
        }
    }
}
