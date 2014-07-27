using System;
using Aws.Interfaces.Services;
using Aws.Services;
using Sugar;
using Sugar.Command;

namespace Aws.Commands
{
    /// <summary>
    /// Command to list all available hosted zones in Route 53.
    /// </summary>
    public class ListHostedZones : BoundCommand<ListHostedZones.Options>
    {
        [Flag("list", "zones")]
        public class Options
        {
            [Parameter("region")]
            public string Region { get; set; }
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
        /// Initializes a new instance of the <see cref="ListHostedZones"/> class.
        /// </summary>
        public ListHostedZones()
        {
            Route53Service = new Route53Service();
        }

        /// <summary>
        /// Executes the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int Execute(Options options)
        {
            var endPoint = Route53Service.ToRegionEndPoint(options.Region);

            var zones = Route53Service.ListHostedZones(endPoint);

            var table = new TextTable("Id", "Zone");
            table.AddSeperator();

            foreach (var zone in zones)
            {
                table.AddRow(zone.Id, zone.Name);
            }

            Console.Write(table.ToString());

            return (int) ExitCode.Success;
        }
    }
}
