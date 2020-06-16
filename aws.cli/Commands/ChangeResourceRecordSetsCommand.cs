using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Route53;
using Amazon.Route53.Model;
using Comsec.Aws.Services;
using Sugar.Net;

namespace Comsec.Aws.Commands
{
    /// <summary>
    /// Command to change a record in a give hosted zone.
    /// </summary>
    public class ChangeResourceRecordSetsCommand : ICommand<ChangeResourceRecordSetsCommand.Input>
    {
        private readonly IRoute53Service route53Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeResourceRecordSetsCommand"/> class.
        /// </summary>
        public ChangeResourceRecordSetsCommand(IRoute53Service route53Service)
        {
            this.route53Service = route53Service;
        }

        public class Input
        {
            public Input(string host, string ip, bool privateIp, bool publicIp, int ttl)
            {
                Host = host;
                IpAddress = ip;
                GetLocalIp = privateIp;
                GetPublicIp = publicIp;
                Ttl = ttl;
            }

            public string Host { get; set; }

            public string IpAddress { get; set; }
            
            public bool GetLocalIp { get; set; }

            public bool GetPublicIp { get; set; }

            public int Ttl { get; set; }
        }

        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public async Task Execute(Input input)
        {
            var create = false;
            var domain = new DomainName(input.Host).DomainSansSubDomain;

            var zone = await route53Service.GetZone(domain);

            if (zone == null)
            {
                throw new ApplicationException($"Unable to load {domain}");
            }

            var allRecords = await route53Service.ListResourceRecordSets(zone.Id);

            var match =
                allRecords.FirstOrDefault(
                    r => r.Name.StartsWith(input.Host, StringComparison.InvariantCultureIgnoreCase));

            if (match == null)
            {
                match = new ResourceRecordSet
                         {
                             Name = input.Host.ToLower(),
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

            if (input.Ttl > 0)
            {
                change.TTL = input.Ttl;
            }

            if (!string.IsNullOrEmpty(input.IpAddress))
            {
                change.ResourceRecords.Clear();

                var value = new ResourceRecord {Value = input.IpAddress};

                change.ResourceRecords.Add(value);
            }
            else if (input.GetPublicIp)
            {
                change.ResourceRecords.Clear();

                var value = new ResourceRecord {Value = route53Service.GetPublicIpAddress()};

                change.ResourceRecords.Add(value);
            }
            else if (input.GetLocalIp)
            {
                change.ResourceRecords.Clear();

                var value = new ResourceRecord {Value = route53Service.GetLocalIpAddress()};

                change.ResourceRecords.Add(value);
            }

            if (change.ResourceRecords.Count == 0)
            {
                throw new ApplicationException("Nothing to change, check your parameters");
            }
            
            if (create)
            {
                await route53Service.CreateResourceRecordSet(zone.Id, change);

                Console.WriteLine("Created A record {0} with value: {1} (TTL: {2})", change.Name,
                    change.ResourceRecords.First().Value, change.TTL);
            }
            else
            {
                await route53Service.ReplaceResourceRecordSet(zone.Id, match, change);

                Console.WriteLine("Updated A record {0} with value: {1} (TTL: {2})", change.Name,
                    change.ResourceRecords.First().Value, change.TTL);
            }
        }
    }
}
