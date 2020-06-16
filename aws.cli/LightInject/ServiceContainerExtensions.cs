using System;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using LightInject;

namespace Comsec.Aws.LightInject
{
    public static class ServiceContainerExtensions
    {
        public static ServiceContainer RegisterOptionalAwsRegion(this ServiceContainer container, string regionName)
        {
            // Region override
            if (!string.IsNullOrEmpty(regionName))
            {
                var region = RegionEndpoint.GetBySystemName(regionName);

                container.RegisterInstance<RegionEndpoint>(region);
            }

            return container;
        }

        public static ServiceContainer RegisterOptionalAwsCredentials(this ServiceContainer container, string profileName, string profilesLocation)
        {
            // Credentials
            if (!string.IsNullOrEmpty(profileName))
            {
                if (string.IsNullOrEmpty(profilesLocation))
                {
                    var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                    // Assumes the credentials are in ~\.aws\credentials
                    profilesLocation = Path.Combine(userFolder, @".aws\credentials");
                }

                var chain = new CredentialProfileStoreChain(profilesLocation);

                if (!chain.TryGetAWSCredentials(profileName, out var credentials))
                {
                    throw new AmazonClientException("Unable to initialise AWS credentials from profile name");
                }

                container.RegisterInstance<AWSCredentials>(credentials);
            }

            return container;
        }
    }
}