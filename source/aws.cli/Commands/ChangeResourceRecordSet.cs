using System;
using System.Linq;
using Amazon.Domain;
using Amazon.Services;
using Comsec.Sugar.Command;
using Comsec.Sugar.Net;

namespace Amazon.Commands
{
    /// <summary>
    /// Lists all the hosted zones
    /// </summary>
    public class ChangeResourceRecordSets : ICommand
    {
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
        /// Determines whether this instance can execute the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute the specified parameters; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute(Parameters parameters)
        {
            return !string.IsNullOrEmpty(parameters.AsString("--set"));
        }

        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void Execute(Parameters parameters)
        {
            var host = parameters.AsString("--set");

            var domain = new DomainName(host).DomainSansSubDomain;

            var zone = Route53Service.GetZone(domain);

            if (zone == null)
            {
                Console.WriteLine("Unable to load {0}", domain);
                return;
            }

            var record = Route53Service
                .ListResourceRecordSets(zone.Id)
                .FirstOrDefault(r => string.Compare(host, r.Name, true) == 0);

            if (record == null)
            {
                Console.WriteLine("Unable to load {0}", host);
                return;   
            }

            var change = (ResourceRecordSet) record.Clone();

            change.ResourceRecords.Clear();
            change.ResourceRecords.Add(parameters.AsString("--ip", record.ResourceRecords[0]));

            change.TTL = parameters.AsInteger("--ttl", record.TTL);

            if (parameters.Contains("--my-ip"))
            {
                change.ResourceRecords.Clear();
                change.ResourceRecords.Add(Route53Service.GetPublicIpAddress());
            }

            Route53Service.ChangeResourceRecordSet(zone.Id, record, change);
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get { return "--list   Returns a list of zones hosted within Route 53"; }
        }

        /// <summary>
        /// Gets the help.
        /// </summary>
        public string Help
        {
            get { return "--list   Returns a list of zones hosted within Route 53"; }
        }
    }
}
