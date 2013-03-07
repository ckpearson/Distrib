using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Shared.Events
{
    public sealed class NodeListeningChangedEvent
    {
        private readonly bool _isListening;

        public NodeListeningChangedEvent(bool isListening)
        {
            _isListening = isListening;
        }

        public bool IsListening
        {
            get
            {
                return _isListening;
            }
        }
    }
}
