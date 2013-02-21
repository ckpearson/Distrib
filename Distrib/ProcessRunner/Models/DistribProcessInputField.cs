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
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.Models
{
    public sealed class DistribProcessInputField : IDataErrorInfo, INotifyPropertyChanged
    {
        private IProcessJobValueField _field;

        public DistribProcessInputField(IProcessJobValueField field)
        {
            _field = field;
        }

        public IProcessJobDefinitionField Definition
        {
            get
            {
                return _field.Definition;
            }
        }

        public IProcessJobValueField UnderlyingValueField
        {
            get { return _field; }
        }

        public object Value
        {
            get { return _field.Value; }
            set
            {
                _field.Value = value;
                OnPropChange();
            }
        }

        public string Error
        {
            get;
            private set;
        }

        public string this[string columnName]
        {
            get
            {
                Error = null;

                if (columnName == "Value")
                {
                    // validate the value that has been set

                    if (_field.Definition.Config.HasDefaultValue == false &&
                        Value == null)
                    {
                        // No default value
                        Error = string.Format("{0} has no default value, so a value must be provided.",
                            _field.Definition.Name);
                        return Error;
                    }

                    try
                    {
                        _field.Value = Convert.ChangeType(_field.Value, _field.Definition.Type);
                    }
                    catch
                    {
                        // An exception happend because the string value provided in the interface
                        // can't be converted across to the requested input type, so this is a validation "error"

                        Error = string.Format("The value of '{0}' could not be used for this field, as it " +
                            "could not be converted to the required type of '{1}'",
                            _field.Value == null ? "NULL" : _field.Value.ToString(),
                            _field.Definition.Type.Name);
                        return Error;
                    }
                }
                return Error;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChange([CallerMemberName] string prop = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
