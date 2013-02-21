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
            this.DisplayName = config.DisplayName;
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
