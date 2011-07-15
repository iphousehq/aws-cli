using System.Collections.Generic;

namespace Amazon.Domain
{
    /// <summary>
    /// Represents a Hosted Zone
    /// </summary>
    public class HostedZone : HostedZoneDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostedZone"/> class.
        /// </summary>
        public HostedZone()
        {
            Nameservers = new List<string>();
        }

        /// <summary>
        /// Gets or sets the nameservers.
        /// </summary>
        /// <value>
        /// The nameservers.
        /// </value>
        public IList<string> Nameservers { get; set; }
    }
}
