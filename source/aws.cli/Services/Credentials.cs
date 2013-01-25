using System;
using Sugar.Command;

namespace Amazon.Services
{
    /// <summary>
    /// Class to represent the Amazon Credentials
    /// </summary>
    public class Credentials : ICredentials
    {
        /// <summary>
        /// Gets the access key id.
        /// </summary>
        public string AccessKeyId
        {
            get
            {
                var arguments = new Parameters(Environment.CommandLine);

                if (arguments.Contains("-o"))
                {
                    return arguments.AsString("-o");
                }

                return Environment.GetEnvironmentVariable("EC2_ACCESS_KEY");
            }
        }

        /// <summary>
        /// Gets the secret access key.
        /// </summary>
        public string SecretAccessKey
        {
            get
            {
                var arguments = new Parameters(Environment.CommandLine);

                if (arguments.Contains("-w"))
                {
                    return arguments.AsString("-w");
                }

                return Environment.GetEnvironmentVariable("EC2_SECRET_ACCESS_KEY");
            }
        }
    }
}
