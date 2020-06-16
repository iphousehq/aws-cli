using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Route53.Model;
using Comsec.Aws.Services;
using LightInject;
using Moq;
using NUnit.Framework;

namespace Comsec.Aws.Commands
{
    [TestFixture]
    [Parallelizable]
    public class ListHostedZonesTest
    {
        public ListHostedZonesCommand Setup(out MockingServiceContainer context)
        {
            context = new MockingServiceContainer();

            context.Register<ListHostedZonesCommand>();

            return context.GetInstance<ListHostedZonesCommand>();
        }

        [Test]
        public async Task TestListHostedZones()
        {
            var command = Setup(out var context);

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.ListHostedZones())
                   .ReturnsAsync(new List<HostedZone>());

            await command.Execute();
        }
    }
}
