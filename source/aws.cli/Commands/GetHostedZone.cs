using System;
using Amazon.Services;
using Sugar.Command;

namespace Amazon.Commands
{
    /// <summary>
    /// Lists all the hosted zones
    /// </summary>
    public class GetHostedZone : ICommand
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
        public GetHostedZone()
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
            return parameters.Contains("--get");
        }

        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public int Execute(Parameters parameters)
        {
            var id = parameters.AsString("--get");

            var zone = Route53Service.GetHostedZone(id);

            if (zone == null)
            {
                Console.WriteLine("Couldn't find the zone with the id {0}.", id);
                return 0;
            }

            Console.WriteLine("Name        : {0}", zone.Name);
            Console.WriteLine("Id          : {0}", zone.Id);
            Console.WriteLine("Description : {0}", zone.CallerReference);

            foreach (var nameserver in zone.Nameservers)
            {
                Console.WriteLine("Nameserver  : {0}", nameserver);                
            }

            return 0;
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
