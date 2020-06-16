using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Route53;
using Amazon.Route53.Model;
using Sugar.Net;

namespace Comsec.Aws.Services
{
    /// <summary>
    /// Service to wrap call to the Amazon Route 53 API.
    /// </summary>
    public class Route53Service : IRoute53Service
    {
        private readonly IHttpService httpService;
        private readonly IAmazonRoute53 client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Route53Service"/> class.
        /// </summary>
        public Route53Service(IHttpService httpService, IAmazonRoute53 client)
        {
            this.httpService = httpService;
            this.client = client;
        }
        
        /// <summary>
        /// Gets the meta instance metadata.
        /// </summary>
        /// <param name="key">The key (e.g. public-ipv4.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unable to determine meta data from: http://169.254.169.254/latest/meta-data/ +
        ///                                         key</exception>
        public string GetMetaInstanceMetadata(string key)
        {
            // This only works if you run this utility on an EC2 instance
            var html = httpService.Get("http://169.254.169.254/latest/meta-data/" + key, "");

            if (html.Success)
            {
                return html.ToString();
            }

            throw new ArgumentException("Unable to determine meta data from: http://169.254.169.254/latest/meta-data/" + key);
        }

        /// <summary>
        /// Gets the public ip address from the instance meta data HTTP API.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">Unable to determine public IP address from: http://169.254.169.254/latest/meta-data/public-ipv4</exception>
        public string GetPublicIpAddress()
        {
            return GetMetaInstanceMetadata("public-ipv4");
        }

        /// <summary>
        /// Gets the local ip address.
        /// </summary>
        /// <returns></returns>
        public string GetLocalIpAddress()
        {
            return GetMetaInstanceMetadata("local-ipv4");
        }

        /// <summary>
        /// Lists the hosted zones.
        /// </summary>
        /// <returns></returns>
        public async Task<List<HostedZone>> ListHostedZones()
        {
            var request = new ListHostedZonesRequest();

            var response = await client.ListHostedZonesAsync(request);

            return response.HostedZones;
        }

        /// <summary>
        /// Gets the zone.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        public async Task<HostedZone> GetZone(string domainName)
        {
            var request = new ListHostedZonesRequest();
            
            var response = await client.ListHostedZonesAsync(request);

            return response.HostedZones
                           .FirstOrDefault(z => z.Name.StartsWith(domainName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Lists the resource record sets in the specified hosted zone.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <returns></returns>
        public async Task<List<ResourceRecordSet>> ListResourceRecordSets(string hostedZoneId)
        {
            var request = new ListResourceRecordSetsRequest
                          {
                              HostedZoneId = hostedZoneId
                          };
            
            var response = await client.ListResourceRecordSetsAsync(request);

            return response.ResourceRecordSets;
        }

        /// <summary>
        /// Creates the resource record set.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        public async Task<ChangeInfo> CreateResourceRecordSet(string hostedZoneId, ResourceRecordSet newRecord)
        {
            var changes = new List<Change>
                          {
                              new Change
                              {
                                  Action = ChangeAction.CREATE,
                                  ResourceRecordSet = newRecord
                              }
                          };

            return await SubmitChangeResourceRecordSets(hostedZoneId, changes);
        }

        /// <summary>
        /// Replaces the resource record set.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="oldRecord">The old record.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        public async Task<ChangeInfo> ReplaceResourceRecordSet(string hostedZoneId, ResourceRecordSet oldRecord,
            ResourceRecordSet newRecord)
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

            return await SubmitChangeResourceRecordSets(hostedZoneId, changes);
        }

        /// <summary>
        /// Submits the change request.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <returns></returns>
        private async Task<ChangeInfo> SubmitChangeResourceRecordSets(string hostedZoneId, List<Change> changes)
        {
            var request = new ChangeResourceRecordSetsRequest
                          {
                              HostedZoneId = hostedZoneId,
                              ChangeBatch = new ChangeBatch {Changes = changes}
                          };
            
            var response = await client.ChangeResourceRecordSetsAsync(request);

            return response.ChangeInfo;
        }
    }
}
