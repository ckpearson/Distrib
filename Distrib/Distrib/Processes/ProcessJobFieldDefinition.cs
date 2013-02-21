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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{

    [Serializable()]
    internal class ProcessJobFieldDefinition : IProcessJobDefinitionField
    {
        private readonly object _lock = new object();

        private readonly Type _type;
        private readonly string _name;
        private readonly FieldMode _mode;

        private IProcessJobFieldConfig _config = ProcessJobFieldConfigFactory.CreateConfig();

        public ProcessJobFieldDefinition(Type type, string name, FieldMode mode)
        {
            if (type == null) throw Ex.ArgNull(() => type);
            if (string.IsNullOrEmpty(name)) throw Ex.ArgNull(() => name);

            _type = type;
            _name = name;
            _mode = mode;
        }

        public Type Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public FieldMode Mode
        {
            get { return _mode; }
        }

        public IProcessJobFieldConfig Config
        {
            get
            {
                lock (_lock)
                {
                    return _config;
                }
            }
            protected set
            {
                lock (_lock)
                {
                    _config = value;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                lock (_lock)
                {
                    if (_config.HasDisplayName)
                    {
                        return _config.DisplayName;
                    }
                    else
                    {
                        return _name;
                    }
                }
            }
        }


        public bool Match(IProcessJobDefinitionField field, bool matchConfig = true)
        {
            return AllCChain<bool>
                .If(false, () => this.Name == field.Name, true)
                .ThenIf(() => this.Mode == field.Mode, true)
                .ThenIf(() => this.Type.Equals(field.Type), true)
                .ThenIf(() => this.DisplayName == field.DisplayName, true)
                .ThenIf(() => (matchConfig == true) ? this.Config.Match(field.Config) : true, true)
                .Result;
        }
    }

    [Serializable()]
    internal sealed class ProcessJobFieldDefinition<T> : ProcessJobFieldDefinition, IProcessJobDefinitionField<T>
    {
        public ProcessJobFieldDefinition(string name, FieldMode mode)
            : base(typeof(T), name, mode)
        {
            base.Config = ProcessJobFieldConfigFactory.CreateConfig<T>();
        }

        public new IProcessJobFieldConfig<T> Config
        {
            get { return (IProcessJobFieldConfig<T>)base.Config; }
        }
    }
}
