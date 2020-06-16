using Amazon;
using Amazon.Route53;
using Amazon.Runtime;
using Comsec.Aws.Services;
using LightInject;
using Sugar.Net;

namespace Comsec.Aws.LightInject
{
    public class AwsCliCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IAmazonRoute53>(f =>
            {
                var region = f.TryGetInstance<RegionEndpoint>();

                var credentials = f.TryGetInstance<AWSCredentials>();

                return credentials != null
                    ? region != null
                        ? new AmazonRoute53Client(credentials, region)
                        : new AmazonRoute53Client(credentials)
                    : new AmazonRoute53Client();
            });

            serviceRegistry.Register<IHttpService, HttpService>();
            serviceRegistry.Register<IRoute53Service, Route53Service>();
        }
    }
}
