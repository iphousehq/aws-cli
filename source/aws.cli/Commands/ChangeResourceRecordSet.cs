using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Amazon.Domain;
using Amazon.Services;
using Sugar.Command;
using Sugar.Net;

namespace Amazon.Commands
{
    /// <summary>
    /// Lists all the hosted zones
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

            [Flag("my-internal-ip")]
            public bool SetMyIpAddress { get; set; }

            [Flag("my-ip")]
            public bool SetMyExternalIpAddress { get; set; }
        }

        /// <summary>
        /// Gets or sets the route53 service.
        /// </summary>
        /// <value>
        /// The route53 service.
        /// </value>
        public Route53Service Route53Service { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListHostedZones"/> class.
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
                return 0;
            }
            var record = Route53Service
                .ListResourceRecordSets(zone.Id)
                .FirstOrDefault(r => String.Compare(options.Host, r.Name, StringComparison.OrdinalIgnoreCase) == 0);

            if (record == null)
            {
                record = new ResourceRecordSet
                {
                    Name = options.Host.ToLower(),
                    TTL = 86400
                };

                create = true;
            }

            var change = (ResourceRecordSet) record.Clone();

            if (options.Ttl > 0)
            {
                change.TTL = options.Ttl;
            }

            if (!string.IsNullOrEmpty(options.IpAddress))
            {
                change.ResourceRecords.Clear();
                change.ResourceRecords.Add(options.IpAddress);                
            }

            else if (options.SetMyExternalIpAddress)
            {
                change.ResourceRecords.Clear();
                change.ResourceRecords.Add(Route53Service.GetPublicIpAddress());
            }

            else if (options.SetMyIpAddress)
            {
                change.ResourceRecords.Clear();
                change.ResourceRecords.Add(GetLocalIpAddress());
            }

            if (create)
            {
                Route53Service.CreateResourceRecordSet(zone.Id, change);
            }
            else
            {
                Route53Service.ChangeResourceRecordSet(zone.Id, record, change);                
            }

            return 0;
        }

        public string GetLocalIpAddress()
        {
            var localIp = string.Empty;
            
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                }
            }

            return localIp;
        }
    }
}
