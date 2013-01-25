using System;
using Amazon.Services;
using Sugar;
using Sugar.Command;

namespace Amazon.Commands
{
    /// <summary>
    /// Lists all the hosted zones
    /// </summary>
    public class ListHostedZones : BoundCommand<ListHostedZones.Options>
    {
        [Flag("list", "zones")]
        public class Options {}

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
        public ListHostedZones()
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
            var zones = Route53Service.ListHostedZones();

            var table = new TextTable("Id", "Zone");
            table.AddSeperator();

            foreach (var zone in zones)
            {
                table.AddRow(zone.Id, zone.Name);
            }

            Console.Write(table.ToString());

            return Success();
        }
    }
}
