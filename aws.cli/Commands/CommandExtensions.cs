using System.CommandLine;
using System.CommandLine.Invocation;
using Comsec.Aws.LightInject;
using LightInject;

namespace Comsec.Aws.Commands
{
    public static class CommandExtensions
    {
        /// <summary>
        /// Defines a sub command, its handler and adds it to the <see cref="parent"/> command.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="container"></param>
        /// <returns>The <see cref="parent" /> for method chaining.</returns>
        public static Command ConfigureListZonesCommand(this Command parent, ServiceContainer container)
        {
            var command = new Command("zones", "Lists all the R53 hosted zones")
            {
                Handler = CommandHandler.Create<string, string, string>(async (profile, profilesLocation, region) =>
                {
                    container.RegisterOptionalAwsCredentials(profile, profilesLocation)
                             .RegisterOptionalAwsRegion(region);

                    container.Register<ListHostedZonesCommand>();

                    var instance = container.GetInstance<ListHostedZonesCommand>();

                    await instance.Execute();
                })
            }.AddAwsSdkCredentialsOptions();

            parent.Add(command);

            return parent;
        }

        public static Command ConfigureListRecordsForZoneCommand(this Command parent, ServiceContainer container)
        {
            var command = new Command("zone", "Lists all the resource records for a given hosted zone")
            {
                new Argument<string>("zoneId", "The R53 zone ID")
            }.AddAwsSdkCredentialsOptions();

            command.Handler = CommandHandler.Create<ListResourceRecordSetsCommand.Input, string, string, string>(
                async (input, profile, profilesLocation, region) =>
                {
                    container.RegisterOptionalAwsCredentials(profile, profilesLocation)
                             .RegisterOptionalAwsRegion(region);

                    container.Register<ListResourceRecordSetsCommand>();

                    var instance = container.GetInstance<ListResourceRecordSetsCommand>();

                    await instance.Execute(input);
                });

            parent.Add(command);

            return parent;
        }

        /// <summary>
        /// Defines a sub command, its handler and adds it to the <see cref="parent"/> command.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="container"></param>
        /// <returns>The <see cref="parent" /> for method chaining.</returns>
        public static Command ConfigureSetCommand(this Command parent, ServiceContainer container)
        {
            var command = new Command("set", "Changes a given R53 record")
            {
                new Argument<string>("host", "The R53 hostname to modify"),
                new Option<string>("--ip",
                    () => "",
                    "The IP address to assign to the R53 record (A record)"),
                new Option<bool>("--privateIp",
                    getDefaultValue: () => false,
                    "A flag to instruct the tool to resolve its private IP and set it as the A record."),
                new Option<bool>("--publicIp",
                    getDefaultValue: () => false,
                    "A flag to instruct the tool to resolve its public IP and set it as the A record."),
                new Option<int>("--ttl",
                    getDefaultValue: () => 0,
                    "When set, changes the \"Time to Live\"")
            }.AddAwsSdkCredentialsOptions();

            command.Handler = CommandHandler.Create<ChangeResourceRecordSetsCommand.Input, string, string, string>(
                async (input, profile, profilesLocation, region) =>
                {
                    container.RegisterOptionalAwsCredentials(profile, profilesLocation)
                             .RegisterOptionalAwsRegion(region);

                    container.Register<ChangeResourceRecordSetsCommand>();

                    var instance = container.GetInstance<ChangeResourceRecordSetsCommand>();

                    await instance.Execute(input);
                });

            parent.Add(command);

            return parent;
        }

        public static Command AddAwsSdkCredentialsOptions(this Command command)
        {
            command.AddOption(new Option<string>("--profile",
                getDefaultValue: () => "",
                description: "The name of the AWS profile to use"));

            command.AddOption(new Option<string>("--profiles-location",
                getDefaultValue: () => "",
                description: "The path to the folder containing the AWS profiles"));
            
            command.AddOption(new Option<string>("--region",
                getDefaultValue: () => "",
                description: "The name of the AWS region to use"));

            return command;
        }
    }
}