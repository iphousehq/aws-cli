namespace Amazon.Services
{
    public interface ICredentials
    {
        /// <summary>
        /// Gets the access key id.
        /// </summary>
        string AccessKeyId { get; }

        /// <summary>
        /// Gets the secret access key.
        /// </summary>
        string SecretAccessKey { get; }

        /// <summary>
        /// Gets a value indicating whether these <see cref="ICredentials" /> have been set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if set; otherwise, <c>false</c>.
        /// </value>
        bool Set { get; }
    }
}