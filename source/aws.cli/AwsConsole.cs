using System;
using Aws.Commands;
using Sugar.Command;

namespace Aws
{
    /// <summary>
    /// Amazon Web Service Console
    /// </summary>
    public class AwsConsole : BaseCommandConsole
    {
        public AwsConsole()
        {
            Commands.Add(new ListHostedZones());
            Commands.Add(new ListResourceRecordSets());
            Commands.Add(new ChangeResourceRecordSets());
        }

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public override int Default()
        {
            Console.WriteLine();
            Console.WriteLine(" AWS CLI Route 53 companion tool");
            Console.WriteLine(" This utility lets you create (or update) A records");
            Console.WriteLine();
            Console.WriteLine(" Documentation: https://github.com/comsechq/aws-cli");
            Console.WriteLine();
            Console.WriteLine(" Usage:");
            Console.WriteLine();
            Console.WriteLine("  aws.exe [options]");
            Console.WriteLine();
            Console.WriteLine(" Options:");
            Console.WriteLine();
            Console.WriteLine("  -list -zones");
            Console.WriteLine("  -list -zone-id value");
            Console.WriteLine("  -set -host [subdomain] [-ip [value]|-public-ip|-local-ip]] -ttl [ttl]");
            Console.WriteLine();
            Console.WriteLine(" To use a different Amazon CLI profile add the following parameter");
            Console.WriteLine();
            Console.WriteLine(" -profile-name [value]");
            Console.WriteLine(" -profiles-location [pathToConfigFile]");
            Console.WriteLine(" -region [value]");
            Console.WriteLine();
            Console.WriteLine(" Default values (unless overriden in `aws.config` file):");
            Console.WriteLine();
            Console.WriteLine(" - Profile name: `default`");
            Console.WriteLine(" - Profiles locations: `~\\.aws\\config` (current user folder)");
            Console.WriteLine(" - Region: `eu-west-1`");
            Console.WriteLine();
            Console.WriteLine(" Disclaimer: Use at your own risk.");

            return (int)ExitCode.NoCommand;
        }
    }
}
