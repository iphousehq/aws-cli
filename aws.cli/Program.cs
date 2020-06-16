using System.CommandLine;
using System.CommandLine.Builder;
using Comsec.Aws.Commands;
using Comsec.Aws.LightInject;
using LightInject;

namespace Comsec.Aws
{
    static class Program
    {
        /// <summary>
        /// Starting point for the program logic
        /// </summary>
        /// <param name="args">The args.</param>
        static int Main(string[] args)
        {
            var options = new ContainerOptions {EnablePropertyInjection = false};

            var container = new ServiceContainer(options);

            container.RegisterFrom<AwsCliCompositionRoot>();

            var rootCommand = new RootCommand("Comsec AWS CLI companion tool")
            {
                new Command("r53", "R53 commands")
                {
                    new Command("list", "Lists R53 resources").ConfigureListZonesCommand(container)
                                                              .ConfigureListRecordsForZoneCommand(container)
                }.ConfigureSetCommand(container)
            };

            var builder = new CommandLineBuilder(rootCommand);

            builder.UseDefaults();
            builder.Build();

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
