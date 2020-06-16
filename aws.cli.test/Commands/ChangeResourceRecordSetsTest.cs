using System;
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
    public class ChangeResourceRecordSetsTest
    {
        public ChangeResourceRecordSetsCommand Setup(out MockingServiceContainer context)
        {
            context = new MockingServiceContainer();

            context.Register<ChangeResourceRecordSetsCommand>();

            return context.GetInstance<ChangeResourceRecordSetsCommand>();
        }

        [Test]
        public void TestExecuteWhenNothingToSetAndZoneDoesNotExist()
        {
            var command = Setup(out var context);

            var input = new ChangeResourceRecordSetsCommand.Input("sub.domain.com", null, 0, false, false);

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetZone("domain.com"))
                   .ReturnsAsync((HostedZone) null);
            
            Assert.ThrowsAsync<ApplicationException>(() => command.Execute(input));
        }

        [Test]
        public void TestExecuteWhenNothingToSetAndZoneExists()
        {
            var command = Setup(out var context);

            var input = new ChangeResourceRecordSetsCommand.Input("sub.domain.com", null, 0, false, false);

            var hostedZone = new HostedZone {Id = "AAAAAAAAA1234"};

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetZone("domain.com"))
                   .ReturnsAsync(hostedZone);

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                   .ReturnsAsync(new List<ResourceRecordSet>());

            Assert.ThrowsAsync<ApplicationException>(() => command.Execute(input));

            context.GetMock<IRoute53Service>()
                   .Verify(call => call.CreateResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>()),
                       Times.Never());

            context.GetMock<IRoute53Service>()
                   .Verify(
                       call => call.ReplaceResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>(),
                           It.IsAny<ResourceRecordSet>()), Times.Never());
        }

        [Test]
        public void TestExecuteWhenSettingTtlOnlyAndNewRecordButNoIpProvided()
        {
            var command = Setup(out var context);

            var input = new ChangeResourceRecordSetsCommand.Input("sub.domain.com", null, 300, false, false);

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetZone("domain.com"))
                   .ReturnsAsync(hostedZone);

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                   .ReturnsAsync(new List<ResourceRecordSet>());

            Assert.ThrowsAsync<ApplicationException>(() => command.Execute(input));

            context.GetMock<IRoute53Service>()
                   .Verify(call => call.CreateResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>()),
                       Times.Never());

            context.GetMock<IRoute53Service>()
                   .Verify(
                       call => call.ReplaceResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>(),
                           It.IsAny<ResourceRecordSet>()), Times.Never());
        }

        [Test]
        public async Task TestExecuteWhenSettingTtlOnlyAndNewRecordWithValue()
        {
            var command = Setup(out var context);

            var input = new ChangeResourceRecordSetsCommand.Input("sub.domain.com", "10.1.2.3", 300, false, false);

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetZone("domain.com"))
                   .ReturnsAsync(hostedZone);

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                   .ReturnsAsync(new List<ResourceRecordSet>());

            await command.Execute(input);

            context.GetMock<IRoute53Service>()
                   .Verify(call => call.CreateResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>()),
                       Times.Once());
        }

        [Test]
        public async Task TestExecuteWhenSettingPublicIpAddress()
        {
            var command = Setup(out var context);

            var input = new ChangeResourceRecordSetsCommand.Input("sub.domain.com",null, 300, false, true);

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetZone("domain.com"))
                   .ReturnsAsync(hostedZone);

            var match = new ResourceRecordSet {Name = "sub.domain.com"};

            var records = new List<ResourceRecordSet> {match};

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                   .ReturnsAsync(records);

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetPublicIpAddress())
                   .Returns("10.1.3.3");

            await command.Execute(input);

            context.GetMock<IRoute53Service>()
                   .Verify(call => call.ReplaceResourceRecordSet("AAAAAAAAA1234", match, It.IsAny<ResourceRecordSet>()), Times.Once());
        }

        [Test]
        public async Task TestExecuteWhenSettingLocalIpAddress()
        {
            var command = Setup(out var context);

            var input = new ChangeResourceRecordSetsCommand.Input("sub.domain.com", null, 300, true, false);

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetZone("domain.com"))
                   .ReturnsAsync(hostedZone);

            var match = new ResourceRecordSet { Name = "sub.domain.com" };

            var records = new List<ResourceRecordSet> { match };

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                   .ReturnsAsync(records);

            context.GetMock<IRoute53Service>()
                   .Setup(call => call.GetLocalIpAddress())
                   .Returns("172.16.3.3");

            await command.Execute(input);

            context.GetMock<IRoute53Service>()
                   .Verify(call => call.ReplaceResourceRecordSet("AAAAAAAAA1234", match, It.IsAny<ResourceRecordSet>()), Times.Once());
        }
    }
}
