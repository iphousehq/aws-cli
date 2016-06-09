using System;
using System.Linq;
using Sugar.Command;

namespace Aws
{
    /// <summary>
    /// Amazon Web Service Console
    /// </summary>
    public class AwsConsole : BaseConsole
    {
        /// <summary>
        /// Entry point for the program logic
        /// </summary>
        protected override int Main()
        {
            var exitCode = Arguments.Count > 0 ? Run(Arguments) : Default();

            return exitCode;
        }

        /// <summary>
        /// Runs the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public int Run(Parameters parameters)
        {
            var exitCode = (int)ExitCode.GeneralError;

            var commandType = new BoundCommandFactory().GetCommandType(parameters,
                () => GetType().Assembly.GetTypes()
                    .Where(type => type.Namespace != null && type.Namespace.StartsWith("Aws.Commands"))
                    .Where(type => type.Name == "Options"));

            if (commandType != null)
            {
                exitCode = Run(commandType, parameters);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("{0:yyyy-MM-dd HH:mm:ss} : ", DateTime.UtcNow);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Unknown command arguments: ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(Arguments);

                Console.ResetColor();
            }

            return exitCode;
        }

        /// <summary>
        /// Runs the specified parameters.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public int Run(Type commandType, Parameters parameters)
        {
            // Assign current parameters
            Parameters.SetCurrent(parameters.ToString());

            var command = (ICommand)Activator.CreateInstance(commandType);

            command.BindParameters(parameters);

            return command.Execute();
        }

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public int Default()
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
            Console.WriteLine(" To use a different credentials or region specify the following parameters");
            Console.WriteLine();
            Console.WriteLine(" -profile [value]");
            Console.WriteLine(" -profiles-location [pathToConfigFile]");
            Console.WriteLine(" -region [value]");
            Console.WriteLine();
            Console.WriteLine(" Disclaimer: Use at your own risk.");

            return (int)ExitCode.NoCommand;
        }
    }
}
