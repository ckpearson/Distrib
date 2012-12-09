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
        /// The plugin has been excluded because the type isn't MarshalByRef making the separation not possible
        /// </summary>
        TypeNotMarshalable,

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