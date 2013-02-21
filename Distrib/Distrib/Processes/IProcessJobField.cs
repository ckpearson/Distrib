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

namespace Distrib.Processes
{
    public interface IProcessJobDefinitionField
    {
        Type Type { get; }
        string Name { get; }
        FieldMode Mode { get; }

        string DisplayName { get; }

        IProcessJobFieldConfig Config { get; }

        bool Match(IProcessJobDefinitionField field, bool matchConfig = true);
    }

    public interface IProcessJobDefinitionField<T> : IProcessJobDefinitionField
    {
        new IProcessJobFieldConfig<T> Config { get; }
    }

    public interface IProcessJobValueField
    {
        IProcessJobDefinitionField Definition { get; }
        object Value { get; set; }
    }

    public interface IProcessJobValueField<T> : IProcessJobValueField
    {
        new IProcessJobDefinitionField<T> Definition { get; }
        new T Value { get; set; }
    }
}
