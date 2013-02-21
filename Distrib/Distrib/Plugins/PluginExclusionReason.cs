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
namespace Distrib.Plugins
{
    /// <summary>
    /// The reason why a plugin was excluded from being used by Distrib
    /// </summary>
    public enum PluginExclusionReason
    {
        /// <summary>
        /// The system has no known reason for excluding, this generally represents a problem in Distrib
        /// if this is seen (assuming the plugin has actually been excluded)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The plugin has been excluded because the identifier for the plugin is in use by a different plugin
        /// within the same assembly
        /// </summary>
        PluginIdentifierNotUnique,

        /// <summary>
        /// The plugin has been excluded owing to the plugin class not implementing the interface
        /// the metadata attribute stated it did.
        /// </summary>
        NonAdherenceToInterface,

        /// <summary>
        /// The plugin has been excluded because the plugin doesn't implement the core distrib plugin interface.
        /// </summary>
        CorePluginInterfaceNotImplemented,

        /// <summary>
        /// The plugin has been excluded because the type isn't CrossAppDomainObject making the separation not possible
        /// </summary>
        TypeNotCrossAppDomainObject,

        /// <summary>
        /// The plugin has been excluded because the controller for the plugin failed validation
        /// </summary>
        PluginControllerInvalid,

        /// <summary>
        /// The plugin has been excluded because the bootstrap process failed
        /// </summary>
        PluginBootstrapFailure,

        /// <summary>
        /// The plugin has been excluded because the additional metadata constraints weren't met
        /// </summary>
        PluginAdditionalMetadataConstraintsNotMet,
    } 
}