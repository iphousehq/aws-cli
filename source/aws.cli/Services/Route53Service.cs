using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.Runtime;
using Aws.Interfaces.Services;
using Sugar.Net;

namespace Aws.Services
{
    /// <summary>
    /// Service to wrap call to the Amazon Route 53 API.
    /// </summary>
    public class Route53Service : IRoute53Service
    {
        #region Depedencies

        /// <summary>
        /// Gets or sets the HTTP service.
        /// </summary>
        /// <value>
        /// The HTTP service.
        /// </value>
        public HttpService HttpService { get; set; } 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Route53Service"/> class.
        /// </summary>
        public Route53Service()
        {
            HttpService = new HttpService();
        }

        /// <summary>
        /// Gets the public ip address from the instace meta data HTTP API.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">Unable to determine public IP address from: http://instance-data/latest/meta-data/public-ipv4</exception>
        public string GetPublicIpAddress()
        {
            // This only works if you run this utility on an EC2 instance
            var html = HttpService.Get("http://instance-data/latest/meta-data/public-ipv4");

            if (html.Success)
            {
                return html.ToString();
            }

            throw new ApplicationException("Unable to determine public IP address from: http://instance-data/latest/meta-data/public-ipv4");
        }

        /// <summary>
        /// Gets the local ip address.
        /// </summary>
        /// <returns></returns>
        public string GetLocalIpAddress()
        {
            var localIp = string.Empty;

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                }
            }

            return localIp;
        }

        /// <summary>
        /// Converts a region string to a region end point.
        /// </summary>
        /// <param name="region">The region (e.g. eu-west-1).</param>
        /// <returns></returns>
        public RegionEndpoint ToRegionEndPoint(string region)
        {
            if (string.IsNullOrEmpty(region))
            {
                region = ConfigurationManager.AppSettings["AWSDefaultRegionEndPoint"];
            }

            return RegionEndpoint.GetBySystemName(region);
        }

        /// <summary>
        /// Initialises the route53 client.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        public AmazonRoute53Client InitialiseRoute53Client(RegionEndpoint region)
        {
            var credentials = new StoredProfileAWSCredentials();

            return new AmazonRoute53Client(credentials, region);
        }

        /// <summary>
        /// Lists the hosted zones.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        public List<HostedZone> ListHostedZones(RegionEndpoint endpoint)
        {
            var client = InitialiseRoute53Client(endpoint);

            var request = new ListHostedZonesRequest();

            var response = client.ListHostedZones(request);

            return response.HostedZones;
        }

        /// <summary>
        /// Gets the zone.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        public HostedZone GetZone(RegionEndpoint endpoint, string domainName)
        {
            var request = new ListHostedZonesRequest();

            var client = InitialiseRoute53Client(endpoint);

            var response = client.ListHostedZones(request);

            return response.HostedZones
                           .FirstOrDefault(
                               z => z.Name.StartsWith(domainName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Lists the resource record sets in the specified hosted zone.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <returns></returns>
        public List<ResourceRecordSet> ListResourceRecordSets(RegionEndpoint endpoint, string hostedZoneId)
        {
            var request = new ListResourceRecordSetsRequest
                          {
                              HostedZoneId = hostedZoneId
                          };

            var client = InitialiseRoute53Client(endpoint);

            var response = client.ListResourceRecordSets(request);

            return response.ResourceRecordSets;
        }

        /// <summary>
        /// Creates the resource record set.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        public ChangeInfo CreateResourceRecordSet(RegionEndpoint endpoint, string hostedZoneId, ResourceRecordSet newRecord)
        {
            var changes = new List<Change>
                          {
                              new Change
                              {
                                  Action = ChangeAction.CREATE,
                                  ResourceRecordSet = newRecord
                              }
                          };

            return SubmitChangeResourceRecordSets(endpoint, hostedZoneId, changes);
        }

        /// <summary>
        /// Replaces the resource record set.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="oldRecord">The old record.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        public ChangeInfo ReplaceResourceRecordSet(RegionEndpoint endpoint, string hostedZoneId, ResourceRecordSet oldRecord, ResourceRecordSet newRecord)
        {
            var changes = new List<Change>
                          {
                              new Change
                              {
                                  Action = ChangeAction.DELETE,
                                  ResourceRecordSet = oldRecord
                              },
                              new Change
                              {
                                  Action = ChangeAction.CREATE,
                                  ResourceRecordSet = newRecord
                              }
                          };

            return SubmitChangeResourceRecordSets(endpoint, hostedZoneId, changes);
        }

        /// <summary>
        /// Submits the change request.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <returns></returns>
        private ChangeInfo SubmitChangeResourceRecordSets(RegionEndpoint endpoint, string hostedZoneId, List<Change> changes)
        {
            var request = new ChangeResourceRecordSetsRequest
                          {
                              HostedZoneId = hostedZoneId,
                              ChangeBatch = new ChangeBatch {Changes = changes}
                          };

            var client = InitialiseRoute53Client(endpoint);

            var response = client.ChangeResourceRecordSets(request);

            return response.ChangeInfo;
        }
    }
}
