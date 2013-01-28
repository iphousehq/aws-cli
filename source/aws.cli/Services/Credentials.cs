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
                string accessKeyId;

                if (Parameters.Current.Contains("o"))
                {
                    accessKeyId = Parameters.Current.AsString("o");
                }
                else
                {
                    accessKeyId = Environment.GetEnvironmentVariable("EC2_ACCESS_KEY");
                }

                if (string.IsNullOrEmpty(accessKeyId))
                {
                    accessKeyId = string.Empty;
                }

                return accessKeyId;
            }
        }

        /// <summary>
        /// Gets the secret access key.
        /// </summary>
        public string SecretAccessKey
        {
            get
            {
                string secretAccessKey;

                if (Parameters.Current.Contains("w"))
                {
                    secretAccessKey = Parameters.Current.AsString("w");
                }
                else
                {
                    secretAccessKey = Environment.GetEnvironmentVariable("EC2_SECRET_ACCESS_KEY");
                }

                if (string.IsNullOrEmpty(secretAccessKey))
                {
                    secretAccessKey = string.Empty;
                }

                return secretAccessKey;
            }
        }

        /// <summary>
        /// Gets a value indicating whether these <see cref="ICredentials" /> have been set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if set; otherwise, <c>false</c>.
        /// </value>
        public bool Set
        {
            get
            {
                return !(string.IsNullOrEmpty(AccessKeyId) && string.IsNullOrEmpty(SecretAccessKey));
            }
        }
    }
}
