namespace Distrib.Plugins
{
    /// <summary>
    /// The reason why a plugin was excluded from being used by Distrib
    /// </summary>
    public enum DistribPluginExlusionReason
    {
        /// <summary>
        /// The system has no known reason for excluding, this generally represents a problem in Distrib
        /// if this is seen (assuming the plugin has actually been excluded)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The plugin has been excluded owing to the plugin class not implementing the interface
        /// the metadata attribute stated it did.
        /// </summary>
        NonAdherenceToInterface,
    } 
}