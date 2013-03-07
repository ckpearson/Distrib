using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Comms
{
    public abstract class CommsEndpointDetails
    {
        private IReadOnlyList<CommsEndpointDetailsField> _fields = null;
        private readonly string _endpointType;

        protected CommsEndpointDetails(string endpointType)
        {
            _endpointType = endpointType;
        }

        protected abstract IEnumerable<CommsEndpointDetailsField> CreateFields();

        public IReadOnlyList<CommsEndpointDetailsField> GetFields()
        {
            if (_fields == null)
            {
                _fields = this.CreateFields().ToList().AsReadOnly();
            }

            return _fields;
        }

        public CommsEndpointDetailsField FieldByName(string name)
        {
            return GetFields().Single(f => f.Name == name);
        }

        public string EndpointType
        {
            get
            {
                return _endpointType;
            }
        }
    }

    public sealed class CommsEndpointDetailsField :
        INotifyPropertyChanged,
        IDataErrorInfo
    {
        private readonly string _name;
        private object _value;

        private object _valueLock = new object();

        private bool _canEdit;

        public CommsEndpointDetailsField(string name, object value, bool canEdit)
        {
            _name = name;
            _value = value;
            _canEdit = canEdit;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CanEdit"));
                }
            }
        }

        public object Value
        {
            get
            {
                lock (_valueLock)
                {
                    return _value;
                }
            }

            set
            {
                lock (_valueLock)
                {
                    _value = value;
                }

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        private Func<object, string> _validationFunc;

        public Func<object, string> ValidationFunc
        {
            get
            {
                return _validationFunc;
            }

            set
            {
                _validationFunc = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _error;
        public string Error
        {
            get { return _error; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName != "Value")
                {
                    _error = null;
                    return null;
                }

                if (_validationFunc == null)
                {
                    _error = null;
                    return null;
                }

                _error = _validationFunc(this.Value);
                return _error;
            }
        }
    }

    public interface ICommsEndpointDetailsField
    {
        string Name { get; }
        object Value { get; }
        bool IsEditable { get; }
    }
}
