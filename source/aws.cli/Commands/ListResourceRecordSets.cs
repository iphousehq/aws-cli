using System;
using Amazon.Services;
using Comsec.Sugar;
using Comsec.Sugar.Command;

namespace Amazon.Commands
{
    /// <summary>
    /// Lists all the hosted zones
    /// </summary>
    public class ListResourceRecordSets : ICommand
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
        public ListResourceRecordSets()
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
            return parameters.Contains("--list") && !string.IsNullOrEmpty(parameters.AsString("--list"));
        }

        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void Execute(Parameters parameters)
        {
            var domain = parameters.AsString("--list");

            var zones = Route53Service.ListHostedZones();

            foreach (var zone in zones)
            {
                if (string.Compare(zone.Name, domain, true) != 0) continue;

                var records = Route53Service.ListResourceRecordSets(zone.Id);

                var table = new TextTable(4);

                table.AddRow("Name", "Type", "TTL", "Value");
                table.AddRow("=");

                foreach (var record in records)
                {
                    table.AddRow(record.Name, record.Type, record.TTL, record.ResourceRecords.ToCsv(", "));
                }
               
                Console.Write(table.ToString());

                return;
            }

            Console.WriteLine("Unable to load record sets for {0}", domain);
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
