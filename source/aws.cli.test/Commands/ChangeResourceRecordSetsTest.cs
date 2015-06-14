using System.Collections.Generic;
using Amazon;
using Amazon.Route53.Model;
using Aws.Interfaces.Services;
using Moq;
using NUnit.Framework;

namespace Aws.Commands
{
    [TestFixture]
    public class ChangeResourceRecordSetsTest
    {
        private ChangeResourceRecordSets command;

        private Mock<IRoute53Service> route53ServiceMock;

        [SetUp]
        public void Setup()
        {
            route53ServiceMock = new Mock<IRoute53Service>();

            command = new ChangeResourceRecordSets
                      {
                          Route53Service = route53ServiceMock.Object
                      };
        }

        [Test]
        public void TestExecuteWhenNothingToSetAndZoneDoesNotExist()
        {
            var options = new ChangeResourceRecordSets.Options
                          {
                              Host = "sub.domain.com"
                          };

            route53ServiceMock
                .Setup(call => call.GetZone("domain.com"))
                .Returns((HostedZone) null);
            
            var result = command.Execute(options);

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void TestExecuteWhenNothingToSetAndZoneExists()
        {
            var options = new ChangeResourceRecordSets.Options
                          {
                              Host = "sub.domain.com"
                          };

            var hostedZone = new HostedZone {Id = "AAAAAAAAA1234"};

            route53ServiceMock
                .Setup(call => call.GetZone("domain.com"))
                .Returns(hostedZone);

            route53ServiceMock
                .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                .Returns(new List<ResourceRecordSet>());

            var result = command.Execute(options);

            route53ServiceMock
                .Verify(call => call.CreateResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>()), Times.Never());

            route53ServiceMock
                .Verify(call => call.ReplaceResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>(), It.IsAny<ResourceRecordSet>()), Times.Never());

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void TestExecuteWhenSettingTtlOnlyAndNewRecordButNoIpProvided()
        {
            var options = new ChangeResourceRecordSets.Options
                          {
                              Host = "sub.domain.com",
                              Ttl = 300
                          };

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            route53ServiceMock
                .Setup(call => call.GetZone("domain.com"))
                .Returns(hostedZone);

            route53ServiceMock
                .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                .Returns(new List<ResourceRecordSet>());

            var result = command.Execute(options);

            route53ServiceMock
                .Verify(call => call.CreateResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>()), Times.Never());

            route53ServiceMock
                .Verify(call => call.ReplaceResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>(), It.IsAny<ResourceRecordSet>()), Times.Never());

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void TestExecuteWhenSettingTtlOnlyAndNewRecordWithValue()
        {
            var options = new ChangeResourceRecordSets.Options
                          {
                              Host = "sub.domain.com",
                              IpAddress = "10.1.2.3",
                              Ttl = 300
                          };

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            route53ServiceMock
                .Setup(call => call.GetZone("domain.com"))
                .Returns(hostedZone);

            route53ServiceMock
                .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                .Returns(new List<ResourceRecordSet>());

            var result = command.Execute(options);

            route53ServiceMock
                .Verify(call => call.CreateResourceRecordSet(It.IsAny<string>(), It.IsAny<ResourceRecordSet>()), Times.Once());

            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestExecuteWhenSettingPublicIpAddress()
        {
            var options = new ChangeResourceRecordSets.Options
                          {
                              Host = "sub.domain.com",
                              GetPublicIp = true,
                              Ttl = 300
                          };

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            route53ServiceMock
                .Setup(call => call.GetZone("domain.com"))
                .Returns(hostedZone);

            var match = new ResourceRecordSet {Name = "sub.domain.com"};

            var records = new List<ResourceRecordSet> {match};

            route53ServiceMock
                .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                .Returns(records);

            route53ServiceMock
                .Setup(call => call.GetPublicIpAddress())
                .Returns("10.1.3.3");

            var result = command.Execute(options);

            route53ServiceMock
                .Verify(call => call.ReplaceResourceRecordSet("AAAAAAAAA1234", match, It.IsAny<ResourceRecordSet>()), Times.Once());

            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestExecuteWhenSettingLocalIpAddress()
        {
            var options = new ChangeResourceRecordSets.Options
                          {
                              Host = "sub.domain.com",
                              GetLocalIp = true,
                              Ttl = 300
                          };

            var hostedZone = new HostedZone { Id = "AAAAAAAAA1234" };

            route53ServiceMock
                .Setup(call => call.GetZone("domain.com"))
                .Returns(hostedZone);

            var match = new ResourceRecordSet { Name = "sub.domain.com" };

            var records = new List<ResourceRecordSet> { match };

            route53ServiceMock
                .Setup(call => call.ListResourceRecordSets("AAAAAAAAA1234"))
                .Returns(records);

            route53ServiceMock
                .Setup(call => call.GetLocalIpAddress())
                .Returns("172.16.3.3");

            var result = command.Execute(options);

            route53ServiceMock
                .Verify(call => call.ReplaceResourceRecordSet("AAAAAAAAA1234", match, It.IsAny<ResourceRecordSet>()), Times.Once());

            Assert.AreEqual(0, result);
        }
    }
}
