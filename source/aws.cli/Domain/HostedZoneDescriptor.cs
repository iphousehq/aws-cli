namespace Amazon.Domain
{
    /// <summary>
    /// Represents a Hosted Zone
    /// </summary>
    public class HostedZoneDescriptor
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the caller reference.
        /// </summary>
        /// <value>
        /// The caller reference.
        /// </value>
        public string CallerReference { get; set; }
    }
}
