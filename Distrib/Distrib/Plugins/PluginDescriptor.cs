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
using Distrib.IOC;
using Distrib.Utils;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    public sealed class PluginDescriptor : IPluginDescriptor
    {
        private readonly string _typeFullName;
        private readonly IPluginMetadata _metadata;
        private readonly string _assemblyPath;

        private readonly WriteOnce<bool> _pluginFoundUsable = new WriteOnce<bool>(false);
        private readonly WriteOnce<PluginExclusionReason> _pluginExclusionReason =
            new WriteOnce<PluginExclusionReason>(PluginExclusionReason.Unknown);
        private readonly WriteOnce<object> _pluginExclusionTag =
            new WriteOnce<object>(null);

        private readonly WriteOnce<IReadOnlyList<IPluginMetadataBundle>> _additionalMetadata =
            new WriteOnce<IReadOnlyList<IPluginMetadataBundle>>(null);

        private readonly object _lock = new object();

        public PluginDescriptor(string typeFullName, IPluginMetadata metadata, string assemblyPath)
        {
            _typeFullName = typeFullName;
            _metadata = metadata;
            _assemblyPath = assemblyPath;
        }

        public string PluginTypeName
        {
            get { return _typeFullName; }
        }

        public IPluginMetadata Metadata
        {
            get { return _metadata; }
        }

        public IReadOnlyList<IPluginMetadataBundle> AdditionalMetadataBundles
        {
            get
            {
                lock (_additionalMetadata)
                {
                    return (!_additionalMetadata.IsWritten) ? null : _additionalMetadata.Value;
                }
            }
        }

        public void SetAdditionalMetadata(IEnumerable<IPluginMetadataBundle> additionalMetadataBundles)
        {
            lock (_additionalMetadata)
            {
                if (!_additionalMetadata.IsWritten)
                {
                    _additionalMetadata.Value = additionalMetadataBundles.ToList().AsReadOnly();
                }
                else
                {
                    throw new InvalidOperationException("Additional metadata already set");
                }
            }
        }

        private T _UsabilitySetRequiredReturn<T>(Func<T> func)
        {
            lock (_lock)
            {
                if (!UsabilitySet)
                {
                    throw new InvalidOperationException("Usability not set yet");
                }
                else
                {
                    return func();
                }
            }
        }

        public bool IsUsable
        {
            get
            {
                return _UsabilitySetRequiredReturn(() => _pluginFoundUsable.Value);
            }
        }

        public PluginExclusionReason ExclusionReason
        {
            get
            {
                return _UsabilitySetRequiredReturn(() => _pluginExclusionReason.Value);
            }
        }

        public object ExclusionTag
        {
            get
            {
                return _UsabilitySetRequiredReturn(() => _pluginExclusionTag.Value);
            }
        }

        public void MarkAsUsable()
        {
            lock (_lock)
            {
                if (!UsabilitySet)
                {
                    _pluginFoundUsable.Value = true;
                    _pluginExclusionReason.Value = PluginExclusionReason.Unknown;
                    _pluginExclusionTag.Value = null;
                }
                else
                {
                    throw new InvalidOperationException("Usability has already been set");
                }
            }
        }

        public void MarkAsUnusable(PluginExclusionReason exclusionReason, object tag = null)
        {
            lock (_lock)
            {
                if (!UsabilitySet)
                {
                    _pluginFoundUsable.Value = false;
                    _pluginExclusionReason.Value = exclusionReason;
                    _pluginExclusionTag.Value = tag;
                }
                else
                {
                    throw new InvalidOperationException("Usability has already been set");
                }
            }
        }

        public bool UsabilitySet
        {
            get
            {
                lock (_lock)
                {
                    return _pluginFoundUsable.IsWritten;
                }
            }
        }


        public string AssemblyPath
        {
            get { return _assemblyPath; }
        }


        public bool Match(IPluginDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");

            try
            {
                lock (_lock)
                {
                    return (this.AssemblyPath == descriptor.AssemblyPath &&
                        this.PluginTypeName == descriptor.PluginTypeName);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine a match", ex);
            }
        }
    }
}
