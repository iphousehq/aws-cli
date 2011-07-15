using System;
using Amazon.Services;
using Comsec.Sugar.Command;

namespace Amazon.Commands
{
    /// <summary>
    /// Lists all the hosted zones
    /// </summary>
    public class ListHostedZones : ICommand
    {
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
        /// Determines whether this instance can execute the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute the specified parameters; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute(Parameters parameters)
        {
            return parameters.Contains("--list") && string.IsNullOrEmpty(parameters.AsString("--list"));
        }

        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void Execute(Parameters parameters)
        {
            var zones = Route53Service.ListHostedZones();

            foreach (var zone in zones)
            {
                Console.WriteLine(zone.Name + " - " + zone.Id);
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get { return "--list   Returns a list of zones hosted within Route 53"; }
        }

        /// <summary>
        /// Gets the help.
        /// </summary>
        public string Help
        {
            get { return "--list   Returns a list of zones hosted within Route 53"; }
        }
    }
}
