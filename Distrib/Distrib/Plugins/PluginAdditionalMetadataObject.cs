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
using System.Reflection;
using Distrib.Utils;
using System.Collections.ObjectModel;

namespace Distrib.Plugins
{
    [Serializable()]
    public abstract class PluginAdditionalMetadataObject
    {
        private readonly Type _interfaceType;
        private readonly string _identity = Guid.NewGuid().ToString();
        private readonly PluginMetadataBundleExistencePolicy _existencePolicy
            = PluginMetadataBundleExistencePolicy.NotImportant;

        private WriteOnce<IReadOnlyList<PropertyInfo>> _listMetadataProperties =
            new WriteOnce<IReadOnlyList<PropertyInfo>>(null);

        private PluginAdditionalMetadataObject() { }
        protected PluginAdditionalMetadataObject(
            Type interfaceType,
            string identity,
            PluginMetadataBundleExistencePolicy existencePolicy)
        {
            _interfaceType = interfaceType;
            _identity = identity;
            _existencePolicy = existencePolicy;
        }

        public Type MetadataInterfaceType
        {
            get { return _interfaceType; }
        }

        public string MetadataIdentity
        {
            get { return _identity; }
        }

        public PluginMetadataBundleExistencePolicy MetadataExistencePolicy
        {
            get { return _existencePolicy; }
        }

        private object _doMetadataReturn()
        {
            if (_interfaceType == null) throw new InvalidOperationException("No metadata type has been provided");
            if (!_interfaceType.IsInterface) throw new InvalidOperationException("Metadata type must be an interface");

            try
            {
                var metadataObject = _provideMetadata();

                // Make sure the metadata object is serializable
                if (metadataObject.GetType().GetCustomAttribute<SerializableAttribute>() == null)
                {
                    throw new InvalidOperationException("Metadata concrete class must be serializable");
                }

                // Make sure that the metadata object can actually cast across to the interface type
                if (!_interfaceType.IsAssignableFrom(metadataObject.GetType()))
                {
                    throw new InvalidOperationException("Returned metadata object is not assignable from the metadata interface type");
                }

                return metadataObject;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get metadata", ex);
            }
        }

        /// <summary>
        /// When overriden in a derived class, returns the metadata object holding the additional metadata
        /// </summary>
        /// <returns>The metadata object</returns>
        protected abstract object _provideMetadata();

        /// <summary>
        /// Gets the pure object holding the metadata
        /// </summary>
        /// <returns>The metadata object</returns>
        public object ProvideMetadataInstance()
        {
            return _doMetadataReturn();
        }

        /// <summary>
        /// Gets the metadata object cast to the specified form
        /// </summary>
        /// <typeparam name="T">The metadata interface type</typeparam>
        /// <returns>The metadata interface instance</returns>
        public T ProvideMetadataInstance<T>()
        {
            return (T)_doMetadataReturn();
        }

        /// <summary>
        /// Gets the key-value pairs of the metadata
        /// </summary>
        /// <returns>A dictionary containing the metadata key-value pairs</returns>
        public IReadOnlyDictionary<string, object> ProvideMetadataKVPs()
        {
            var dict = new Dictionary<string, object>();

            try
            {
                var metadataObject = _doMetadataReturn();

                // Grab all the properties with public getters and setters
                _listMetadataProperties.Value =
                    _interfaceType.GetProperties()
                    .Where(p => p.GetGetMethod(false) != null && p.GetSetMethod(false) != null)
                    .ToList()
                    .AsReadOnly();

                // Make sure there are some
                if (_listMetadataProperties.Value == null ||
                    _listMetadataProperties.Value.Count == 0)
                {
                    throw new InvalidOperationException("The metadata type provided has no properties with public getters and setters");
                }

                // Run through and pull out the name and value
                foreach (var pi in _listMetadataProperties.Value)
                {
                    dict.Add(pi.Name, pi.GetValue(metadataObject));
                }

                return new ReadOnlyDictionary<string, object>(dict);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get metadata key-values", ex);
            }
        }
    }
}
