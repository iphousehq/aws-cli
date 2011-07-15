namespace Amazon.Domain
{
    /// <summary>
    /// Enumeration to represent the different type of DNS records in the Amazon
    /// Route 53 DNS system.
    /// </summary>
    public enum ResourceRecordSetType
    {
        /// <summary>
        /// An A Record
        /// </summary>
        A,
        AAAA,
        /// <summary>
        /// A CNAME Record
        /// </summary>
        CNAME,
        /// <summary>
        /// A Mail Exchange (MX) Record
        /// </summary>
        MX,
        NS,
        PTR,
        SOA,
        SPF,
        SRV,
        TXT        
    }
}
