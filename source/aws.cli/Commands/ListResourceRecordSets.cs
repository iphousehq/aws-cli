using System;
using System.Linq;
using Aws.Interfaces.Services;
using Aws.Services;
using Sugar;
using Sugar.Command;

namespace Aws.Commands
{
    /// <summary>
    /// Lists all the records in the given hosted zone
    /// </summary>
    public class ListResourceRecordSets : BoundCommand<ListResourceRecordSets.Options>
    {
        [Flag("list")]
        public class Options
        {
            [Parameter("region")]
            public string Region { get; set; }

            [Parameter("zone-id", Required = true)]
            public string ZoneId { get; set; }
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
        /// Initializes a new instance of the <see cref="ListResourceRecordSets"/> class.
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
            var endPoint = Route53Service.ToRegionEndPoint(options.Region);

            var records = Route53Service.ListResourceRecordSets(endPoint, options.ZoneId);

            if (records != null && records.Any())
            {
                var table = new TextTable("Name", "Type", "TTL", "Value");
                table.AddSeperator();

                foreach (var record in records)
                {
                    table.AddRow(record.Name, record.Type, record.TTL, record.ResourceRecords.ToCsv(", "));
                }

                Console.Write(table.ToString());

                return Success();
            }

            Console.WriteLine("Unable to load record sets for {0}", options.ZoneId);

            return Success();
        }
    }
}
