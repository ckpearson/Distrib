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
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    [Serializable()]
    internal class ProcessJobFieldConfig : IProcessJobFieldConfig, IProcessJobFieldConfig_Internal
    {
        protected readonly object _lock = new object();

        private LockValue<object> _defaultValue = new LockValue<object>(null);

        public object DefaultValue
        {
            get
            {
                return _defaultValue.Value;
            }
            set
            {
                _defaultValue.Value = value;
            }
        }

        public bool HasDefaultValue
        {
            get { return _defaultValue.Value != null; }
        }

        public void Adopt(IProcessJobFieldConfig config)
        {
            this.DefaultValue = config.DefaultValue;
            this.DeferredValueProvider = config.DeferredValueProvider;
        }

        private LockValue<IDeferredValueProvider> _deferredValueProvider = new LockValue<IDeferredValueProvider>(null);
        public IDeferredValueProvider DeferredValueProvider
        {
            get { return _deferredValueProvider.Value; }
            set
            {
                _deferredValueProvider.Value = value;
            }
        }


        public bool HasDeferredValueProvider
        {
            get { return _deferredValueProvider.Value != null; }
        }

        private LockValue<string> _displayName = new LockValue<string>(null);
        public string DisplayName
        {
            get
            {
                return _displayName.Value;
            }
            set
            {
                _displayName.Value = value;
            }
        }

        public bool HasDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName.Value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public bool Match(IProcessJobFieldConfig config)
        {
            return AllCChain<bool>
                .If(false, () => this.DefaultValue == config.DefaultValue, true)
                .ThenIf(() => this.DeferredValueProvider == config.DeferredValueProvider, true)
                .ThenIf(() => this.DisplayName == config.DisplayName, true)
                .Result;
        }
    }

    [Serializable()]
    internal sealed class ProcessJobFieldConfig<T> : ProcessJobFieldConfig, IProcessJobFieldConfig<T>
    {
        public new IDeferredValueProvider<T> DeferredValueProvider
        {
            get { return (IDeferredValueProvider<T>)base.DeferredValueProvider; }
            set { base.DeferredValueProvider = value; }
        }

        public new T DefaultValue
        {
            get
            {
                if (!base.HasDefaultValue)
                {
                    return default(T);
                }
                else
                {
                    return (T)base.DefaultValue;
                }
            }
            set
            {
                base.DefaultValue = value;
            }
        }
    }
}
