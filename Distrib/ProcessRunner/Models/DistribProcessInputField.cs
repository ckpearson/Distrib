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
