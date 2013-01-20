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

namespace Distrib.Plugins
{
    public sealed class PluginCoreUsabilityCheckService : IPluginCoreUsabilityCheckService
    {
        public Res<PluginExclusionReason, object> CheckCoreUsability(IPluginDescriptor descriptor,
            IPluginAssemblyManager assemblyManager)
        {
            var res = PluginExclusionReason.Unknown;
            object resultAddit = null;
            bool success = true;

            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");
            if (assemblyManager == null) throw new ArgumentNullException("Assembly manager must be supplied");

            try
            {
                var _result = CChain<Tuple<PluginExclusionReason, object>>
                    // Check the plugin's identifier is unique
                    .If(() => assemblyManager.GetPluginDescriptors().Count(pd => pd.Metadata.Identifier ==
                        descriptor.Metadata.Identifier) > 1,
                            new Tuple<PluginExclusionReason, object>(PluginExclusionReason.PluginIdentifierNotUnique,
                                // Get all the other type names that are plugins sharing the identifier
                                assemblyManager.GetPluginDescriptors()
                                .Where(d => d.Metadata.Identifier == descriptor.Metadata.Identifier &&
                                    d.PluginTypeName != descriptor.PluginTypeName)
                                .Select(d => d.PluginTypeName)
                                .ToList()
                                .AsReadOnly()))
                    // Check the plugin is marshalable
                    .ThenIf(() => !assemblyManager.PluginTypeIsMarshalable(descriptor),
                        new Tuple<PluginExclusionReason, object>(PluginExclusionReason.TypeNotCrossAppDomainObject, null))
                    // Check it implements the IPlugin interface
                    .ThenIf(() => !assemblyManager.PluginTypeImplementsCorePluginInterface(descriptor),
                        new Tuple<PluginExclusionReason,object>(PluginExclusionReason.CorePluginInterfaceNotImplemented, null))
                    // Check it implements the specific plugin interface it claims to
                    .ThenIf(() => !assemblyManager.PluginTypeImplementsPromisedInterface(descriptor),
                        new Tuple<PluginExclusionReason,object>(PluginExclusionReason.NonAdherenceToInterface, null))
                    .Result;

                if (_result == null)
                {
                    success = true;
                    res = PluginExclusionReason.Unknown;
                    resultAddit = null;
                }
                else
                {
                    success = false;
                    res = _result.Item1;
                    resultAddit = _result.Item2;
                }

                return new Res<PluginExclusionReason, object>((success &= true), res, resultAddit);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to check core usability of plugin", ex);
            }
        }
    }
}
