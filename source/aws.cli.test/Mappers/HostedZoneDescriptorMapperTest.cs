using System.IO;
using NUnit.Framework;

namespace Amazon.Mappers
{
    [TestFixture]
    public class HostedZoneDescriptorMapperTest
    {
        [Test]
        public void TestMapThreeItems()
        {
            var xml = File.ReadAllText("../../Samples/ListHostedZones.xml");

            var results = new HostedZoneDescriptorMapper().Map(xml);

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("Q31PM55OG1BPVA", results[0].Id);
            Assert.AreEqual("example.co.uk", results[0].Name);
            Assert.AreEqual("CreateHostedZone, example.co.uk, Wed Jun 15 2011 11:04:26 GMT+0100 (GMT Daylight Time)", results[0].CallerReference);
        }
    }
}
