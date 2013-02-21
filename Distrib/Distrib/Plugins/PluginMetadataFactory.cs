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
using Distrib.IOC;
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

namespace Distrib.Plugins
{
    public sealed class PluginMetadataFactory : CrossAppDomainObject, IPluginMetadataFactory
    {
        private readonly IIOC _ioc;

        public PluginMetadataFactory(IIOC ioc)
        {
            _ioc = ioc;
        }

        public IPluginMetadata CreateMetadata(Type interfaceType, 
            string name, string description, double version, string author, string identifier, Type controllerType)
        {
            return _ioc.Get<IPluginMetadata>(new[]
            {
                new IOCConstructorArgument("interfaceType", interfaceType),
                new IOCConstructorArgument("name", name),
                new IOCConstructorArgument("description", description),
                new IOCConstructorArgument("version", version),
                new IOCConstructorArgument("author", author),
                new IOCConstructorArgument("identifier", identifier),
                new IOCConstructorArgument("controllerType", controllerType),
            });
        }

        public IPluginMetadata CreateMetadataFromPluginAttribute(PluginAttribute attribute)
        {
            return CreateMetadata(
                attribute.InterfaceType,
                attribute.Name,
                attribute.Description,
                attribute.Version,
                attribute.Author,
                attribute.Identifier,
                attribute.ControllerType);
        }
    }
}
