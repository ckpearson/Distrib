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

namespace ProcessNode.Services
{
    public interface IAppStateService
    {
        void UpdateStatus(AppStatusUpdate update);
    }

    public enum StatusLevel
    {
        Ok = 0,
        Warning = 1,
        Error = 2,
    }

    public sealed class AppStatusUpdate
    {
        private readonly string _headline;
        private readonly string _message;
        private readonly string _additonal;

        private readonly List<AppStatusUpdateInteractionOption> _interactions;

        private readonly StatusLevel _statusLevel;

        public AppStatusUpdate(
            string headline,
            string message,
            string additional,
            StatusLevel statusLevel,
            IEnumerable<AppStatusUpdateInteractionOption> interactions = null)
        {
            _headline = headline;
            _message = message;
            _additonal = additional;
            _statusLevel = statusLevel;

            _interactions = interactions == null ? null : interactions.ToList();
        }

        public string Headline { get { return _headline; } }
        public string Message { get { return _message; } }
        public string Additional { get { return _additonal; } }

        public StatusLevel Level { get { return _statusLevel; } }
        public IEnumerable<AppStatusUpdateInteractionOption> InteractionOptions { get { return _interactions; } }
    }

    public sealed class AppStatusUpdateInteractionOption
    {
        private readonly string _message;
        private readonly Action _action;

        public AppStatusUpdateInteractionOption(
            string message,
            Action action)
        {
            _message = message;
            _action = action;
        }

        public string Message { get { return _message; } }
        public Action Action { get { return _action; } }
    }
}
