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
