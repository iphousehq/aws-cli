using System;
using Amazon.Commands;
using Sugar.Command;

namespace Amazon
{
    /// <summary>
    /// Amazon Web Service Console
    /// </summary>
    public class AwsConsole : BaseCommandConsole
    {
        public AwsConsole()
        {
            Commands.Add(new ListHostedZones());
            Commands.Add(new GetHostedZone());
            Commands.Add(new ListResourceRecordSets());
            Commands.Add(new ChangeResourceRecordSets());
        }

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public override int Default()
        {
            Console.WriteLine("Route 53 Console Application");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.WriteLine("  aws.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();
            Console.WriteLine("  -list -zones               Lists all registered zones");
            Console.WriteLine("  -list -zone [domain]       Lists contents of the specified zone");
            Console.WriteLine("  -set -host [subdomain] [-ip [ip]|-my-ip|-my-internal-ip]] -ttl [ttl]");
            Console.WriteLine("                             Lists contents of the specified zone");

            return 0;
        }
    }
}
