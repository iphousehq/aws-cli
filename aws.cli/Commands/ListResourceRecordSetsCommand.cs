using System;
using System.Linq;
using System.Threading.Tasks;
using Comsec.Aws.Services;
using Sugar;
using Sugar.Extensions;

namespace Comsec.Aws.Commands
{
    /// <summary>
    /// Lists all the records in the given hosted zone
    /// </summary>
    public class ListResourceRecordSetsCommand : ICommand<ListResourceRecordSetsCommand.Input>
    {
        private readonly IRoute53Service route53Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResourceRecordSetsCommand"/> class.
        /// </summary>
        public ListResourceRecordSetsCommand(IRoute53Service route53Service)
        {
            this.route53Service = route53Service;
        }

        public class Input
        {
            public Input(string zoneId)
            {
                ZoneId = zoneId;
            }

            public string ZoneId { get; set; }
        }

        /// <summary>
        /// Executes the specified options.
        /// </summary>
        /// <param name="input">The options.</param>
        /// <returns></returns>
        public async Task Execute(Input input)
        {
            var records = await route53Service.ListResourceRecordSets(input.ZoneId);

            if (records != null && records.Any())
            {
                var table = new TextTable("Name", "Type", "TTL", "Value");
                table.AddSeperator();

                foreach (var record in records)
                {
                    table.AddRow(record.Name, record.Type, record.TTL, record.ResourceRecords.ToCsv(", "));
                }

                Console.Write(table.ToString());

                return;
            }

            Console.WriteLine("Unable to load record sets for {0}", input.ZoneId);
        }
    }
}
