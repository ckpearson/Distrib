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