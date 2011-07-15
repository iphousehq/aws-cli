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
    }
}