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
        /// Initialises the route53 client.
        /// </summary>
        /// <returns></returns>
        public AmazonRoute53Client InitialiseRoute53Client()
        {
            // Profile Name
            var profileName = Sugar.Command.Parameters.Current.AsString("profile-name", null);
            if (string.IsNullOrEmpty(profileName))
            {
                profileName = ConfigurationManager.AppSettings["AWSProfileName"];
            }

            // Profiles Location
            var profilesLocation = Sugar.Command.Parameters.Current.AsString("profiles-location", null);
            if (string.IsNullOrEmpty(profilesLocation))
            {
                profilesLocation = ConfigurationManager.AppSettings["AWSProfilesLocation"];
            }
            if (string.IsNullOrEmpty(profilesLocation))
            {
                profilesLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\.aws\config";
            }

            var credentials = new StoredProfileAWSCredentials(profileName, profilesLocation);
            
            // Region
            var regionName = Sugar.Command.Parameters.Current.AsString("region", null);
            if (string.IsNullOrEmpty(regionName))
            {
                regionName = ConfigurationManager.AppSettings["AWSDefaultRegionEndPoint"];
            }

            var region = RegionEndpoint.GetBySystemName(regionName);

            return new AmazonRoute53Client(credentials, region);
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
        /// Lists the hosted zones.
        /// </summary>
        /// <returns></returns>
        public List<HostedZone> ListHostedZones()
        {
            var client = InitialiseRoute53Client();

            var request = new ListHostedZonesRequest();

            var response = client.ListHostedZones(request);

            return response.HostedZones;
        }

        /// <summary>
        /// Gets the zone.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        public HostedZone GetZone(string domainName)
        {
            var request = new ListHostedZonesRequest();

            var client = InitialiseRoute53Client();

            var response = client.ListHostedZones(request);

            return response.HostedZones
                           .FirstOrDefault(
                               z => z.Name.StartsWith(domainName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Lists the resource record sets in the specified hosted zone.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <returns></returns>
        public List<ResourceRecordSet> ListResourceRecordSets(string hostedZoneId)
        {
            var request = new ListResourceRecordSetsRequest
                          {
                              HostedZoneId = hostedZoneId
                          };

            var client = InitialiseRoute53Client();

            var response = client.ListResourceRecordSets(request);

            return response.ResourceRecordSets;
        }

        /// <summary>
        /// Creates the resource record set.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        public ChangeInfo CreateResourceRecordSet(string hostedZoneId, ResourceRecordSet newRecord)
        {
            var changes = new List<Change>
                          {
                              new Change
                              {
                                  Action = ChangeAction.CREATE,
                                  ResourceRecordSet = newRecord
                              }
                          };

            return SubmitChangeResourceRecordSets(hostedZoneId, changes);
        }

        /// <summary>
        /// Replaces the resource record set.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="oldRecord">The old record.</param>
        /// <param name="newRecord">The new record.</param>
        /// <returns></returns>
        public ChangeInfo ReplaceResourceRecordSet(string hostedZoneId, ResourceRecordSet oldRecord, ResourceRecordSet newRecord)
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

            return SubmitChangeResourceRecordSets(hostedZoneId, changes);
        }

        /// <summary>
        /// Submits the change request.
        /// </summary>
        /// <param name="hostedZoneId">The hosted zone identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <returns></returns>
        private ChangeInfo SubmitChangeResourceRecordSets(string hostedZoneId, List<Change> changes)
        {
            var request = new ChangeResourceRecordSetsRequest
                          {
                              HostedZoneId = hostedZoneId,
                              ChangeBatch = new ChangeBatch {Changes = changes}
                          };

            var client = InitialiseRoute53Client();

            var response = client.ChangeResourceRecordSets(request);

            return response.ChangeInfo;
        }
    }
}
