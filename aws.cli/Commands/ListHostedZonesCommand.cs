using System;
using System.Threading.Tasks;
using Comsec.Aws.Services;
using Sugar;

namespace Comsec.Aws.Commands
{
    /// <summary>
    /// Command to list all available hosted zones in Route 53.
    /// </summary>
    public class ListHostedZonesCommand : ICommand
    {
        private readonly IRoute53Service route53Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListHostedZonesCommand"/> class.
        /// </summary>
        public ListHostedZonesCommand(IRoute53Service route53Service)
        {
            this.route53Service = route53Service;
        }

        /// <summary>
        /// Executes the specified options.
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            var zones = await route53Service.ListHostedZones();

            var table = new TextTable("Id", "Zone");
            table.AddSeperator();

            foreach (var zone in zones)
            {
                table.AddRow(zone.Id, zone.Name);
            }

            Console.Write(table.ToString());
        }
    }
}
