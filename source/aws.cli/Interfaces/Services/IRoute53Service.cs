using System.Collections.Generic;
using Amazon.Route53.Model;

namespace Aws.Interfaces.Services
{
    /// <summary>
    /// Interface for abstract access to the AWS route 53 API.
    /// </summary>
    public interface IRoute53Service
    {
        /// <summary>
        /// Gets the public ip address from the instace meta data HTTP API.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">Unable to determine public IP address from: http://instance-data/latest/meta-data/public-ipv4</exception>
        string GetPublicIpAddress();

        /// <summary>
        /// Gets the local ip address.
        /// </summary>
        /// <returns></returns>
        string GetLocalIpAddress();

        /// <summary>
        /// Lists the hosted zones.
        /// </summary>
        /// <returns></returns>
        List<HostedZone> ListHostedZones();

        /// <summary>
        /// Gets the zone.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        HostedZone GetZone(string domainName);

        /// <summary>
        /// Lists the resource record sets in the specified hosted zone.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <returns></returns>
        List<ResourceRecordSet> ListResourceRecordSets(string hostedZoneId);

        /// <summary>
        /// Creates the resource record set.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        ChangeInfo CreateResourceRecordSet(string hostedZoneId, ResourceRecordSet newRecord);

        /// <summary>
        /// Replaces the resource record set.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="oldRecord">The old record.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        ChangeInfo ReplaceResourceRecordSet(string hostedZoneId, ResourceRecordSet oldRecord, ResourceRecordSet newRecord);
    }
}
