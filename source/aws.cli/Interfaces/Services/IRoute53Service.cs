using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Amazon;
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
        /// Converts a region string to a region end point.
        /// </summary>
        /// <param name="region">The region (e.g. eu-west-1).</param>
        /// <returns></returns>
        RegionEndpoint ToRegionEndPoint(string region);

        /// <summary>
        /// Lists the hosted zones.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        List<HostedZone> ListHostedZones(RegionEndpoint endpoint);

        /// <summary>
        /// Gets the zone.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone id.</param>
        /// <returns></returns>
        HostedZone GetZone(RegionEndpoint endpoint, string hostedZoneId);

        /// <summary>
        /// Lists the resource record sets in the specified hosted zone.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <returns></returns>
        List<ResourceRecordSet> ListResourceRecordSets(RegionEndpoint endpoint, string hostedZoneId);

        /// <summary>
        /// Creates the resource record set.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        ChangeInfo CreateResourceRecordSet(RegionEndpoint endpoint, string hostedZoneId, ResourceRecordSet newRecord);

        /// <summary>
        /// Replaces the resource record set.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="oldRecord">The old record.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        ChangeInfo ReplaceResourceRecordSet(RegionEndpoint endpoint, string hostedZoneId, ResourceRecordSet oldRecord, ResourceRecordSet newRecord);
    }
}
