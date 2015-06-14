using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Route53;
using Amazon.Route53.Model;
using Aws.Interfaces.Services;
using Aws.Services;
using Sugar.Command;
using Sugar.Net;

namespace Aws.Commands
{
    /// <summary>
    /// Command to change a record in a give hosted zone.
    /// </summary>
    public class ChangeResourceRecordSets : BoundCommand<ChangeResourceRecordSets.Options>
    {
        [Flag("set")]
        public class Options
        {
            [Parameter("host", Required = true)]
            public string Host { get; set; }

            [Parameter("ip")]
            public string IpAddress { get; set; }

            [Parameter("ttl")]
            public int Ttl { get; set; }

            [Flag("local-ip")]
            public bool GetLocalIp { get; set; }

            [Flag("public-ip")]
            public bool GetPublicIp { get; set; }
        }

        #region Dependencies

        /// <summary>
        /// Gets or sets the route53 service.
        /// </summary>
        /// <value>
        /// The route53 service.
        /// </value>
        public IRoute53Service Route53Service { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeResourceRecordSets"/> class.
        /// </summary>
        public ChangeResourceRecordSets()
        {
            Route53Service = new Route53Service();
        }

        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public override int Execute(Options options)
        {
            var create = false;
            var domain = new DomainName(options.Host).DomainSansSubDomain;

            var zone = Route53Service.GetZone(domain);

            if (zone == null)
            {
                Console.WriteLine("Unable to load {0}", domain);
                return (int) ExitCode.GeneralError;
            }

            var allRecords = Route53Service.ListResourceRecordSets(zone.Id);

            var match =
                allRecords.FirstOrDefault(
                    r => r.Name.StartsWith(options.Host, StringComparison.InvariantCultureIgnoreCase));

            if (match == null)
            {
                match = new ResourceRecordSet
                         {
                             Name = options.Host.ToLower(),
                             TTL = 86400
                         };

                create = true;
            }

            var change = new ResourceRecordSet
                         {
                             Name = match.Name,
                             TTL = match.TTL,
                             Type = RRType.A,
                             ResourceRecords = new List<ResourceRecord>(match.ResourceRecords)
                         };

            if (options.Ttl > 0)
            {
                change.TTL = options.Ttl;
            }

            if (!string.IsNullOrEmpty(options.IpAddress))
            {
                change.ResourceRecords.Clear();

                var value = new ResourceRecord {Value = options.IpAddress};

                change.ResourceRecords.Add(value);
            }
            else if (options.GetPublicIp)
            {
                change.ResourceRecords.Clear();

                var value = new ResourceRecord {Value = Route53Service.GetPublicIpAddress()};

                change.ResourceRecords.Add(value);
            }
            else if (options.GetLocalIp)
            {
                change.ResourceRecords.Clear();

                var value = new ResourceRecord {Value = Route53Service.GetLocalIpAddress()};

                change.ResourceRecords.Add(value);
            }

            if (change.ResourceRecords.Count == 0)
            {
                Console.WriteLine("Nothing to change, check your parameters", domain);
                return (int) ExitCode.GeneralError;
            }
            
            if (create)
            {
                Route53Service.CreateResourceRecordSet(zone.Id, change);

                Console.WriteLine("Created A record {0} with value: {1} (TTL: {2})", change.Name,
                    change.ResourceRecords.First().Value, change.TTL);
            }
            else
            {
                Route53Service.ReplaceResourceRecordSet(zone.Id, match, change);

                Console.WriteLine("Updated A record {0} with value: {1} (TTL: {2})", change.Name,
                    change.ResourceRecords.First().Value, change.TTL);
            }

            return (int)ExitCode.Success;
        }
    }
}
