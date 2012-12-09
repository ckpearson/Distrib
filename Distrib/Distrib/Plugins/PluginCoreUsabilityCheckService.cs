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
                        new Tuple<PluginExclusionReason, object>(PluginExclusionReason.TypeNotMarshalable, null))
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
