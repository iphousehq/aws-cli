using System;
using Amazon.Services;
using Sugar;
using Sugar.Command;

namespace Amazon.Commands
{
    /// <summary>
    /// Lists all the hosted zones
    /// </summary>
    public class ListResourceRecordSets : BoundCommand<ListResourceRecordSets.Options>
    {
        [Flag("list")]
        public class Options
        {
            [Parameter("zone", Required = true)]
            public string Zone { get; set; }
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
        public ListResourceRecordSets()
        {
            Route53Service = new Route53Service();
        }

        /// <summary>
        /// Executes the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public override int Execute(Options options)
        {
            var zones = Route53Service.ListHostedZones();

            foreach (var zone in zones)
            {
                // Choose correct zone
                if (String.Compare(zone.Name, options.Zone, StringComparison.OrdinalIgnoreCase) != 0) continue;

                var records = Route53Service.ListResourceRecordSets(zone.Id);

                var table = new TextTable("Name", "Type", "TTL", "Value");
                table.AddSeperator();

                foreach (var record in records)
                {
                    table.AddRow(record.Name, record.Type, record.TTL, record.ResourceRecords.ToCsv(", "));
                }

                Console.Write(table.ToString());

                return Success();
            }

            Console.WriteLine("Unable to load record sets for {0}", options.Zone);

            return 1;
        }
    }
}
