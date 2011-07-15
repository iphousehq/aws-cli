using System.IO;
using NUnit.Framework;

namespace Amazon.Mappers
{
    [TestFixture]
    public class HostedZoneMapperTest
    {
        [Test]
        public void TestMapThreeItems()
        {
            var xml = File.ReadAllText("../../Samples/GetHostedZone.xml");

            var zone = new HostedZoneMapper().Map(xml);

            Assert.AreEqual("Z31PMW5OR1BPVA", zone.Id);
            Assert.AreEqual("example.com", zone.Name);
            Assert.AreEqual("CreateHostedZone, example.com, Wed Jun 15 2011 11:04:26 GMT+0100 (GMT Daylight Time)", zone.CallerReference);
            Assert.AreEqual(4, zone.Nameservers.Count);
            Assert.AreEqual("ns-1626.awsdns-11.co.uk", zone.Nameservers[0]);
            Assert.AreEqual("ns-262.awsdns-32.com", zone.Nameservers[1]);
            Assert.AreEqual("ns-1482.awsdns-57.org", zone.Nameservers[2]);
            Assert.AreEqual("ns-691.awsdns-22.net", zone.Nameservers[3]);
        }
    }
}
