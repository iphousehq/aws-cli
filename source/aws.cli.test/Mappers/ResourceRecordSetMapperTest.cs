using System.IO;
using Amazon.Domain;
using NUnit.Framework;

namespace Amazon.Mappers
{
    [TestFixture]
    public class ResourceRecordSetMapperTest
    {
        [Test]
        public void TestMapRecords()
        {
            var xml = File.ReadAllText("../../Samples/ListResourceRecordSets.xml");

            var records = new ResourceRecordSetMapper().Map(xml);

            Assert.AreEqual(3, records.Count);
            Assert.AreEqual("example.com", records[0].Name);
            Assert.AreEqual(ResourceRecordSetType.AAAA, records[0].Type);
            Assert.AreEqual("example.com", records[1].Name);
            Assert.AreEqual(ResourceRecordSetType.MX, records[1].Type);
            Assert.AreEqual(1440, records[1].TTL);
            Assert.AreEqual(5, records[1].ResourceRecords.Count);
            Assert.AreEqual("10 ASPMX.L.GOOGLE.COM.", records[1].ResourceRecords[0]);
            Assert.AreEqual("20 ALT1.ASPMX.L.GOOGLE.COM.", records[1].ResourceRecords[1]);
            Assert.AreEqual("20 ALT2.ASPMX.L.GOOGLE.COM.", records[1].ResourceRecords[2]);
            Assert.AreEqual("30 ASPMX2.GOOGLEMAIL.COM.", records[1].ResourceRecords[3]);
            Assert.AreEqual("30 ASPMX3.GOOGLEMAIL.COM.", records[1].ResourceRecords[4]);
            Assert.AreEqual("example.com", records[2].Name);
            Assert.AreEqual(ResourceRecordSetType.NS, records[2].Type);
            Assert.AreEqual(172800, records[2].TTL);
            Assert.AreEqual(4, records[2].ResourceRecords.Count);
            Assert.AreEqual("ns-686.awsdns-21.net.", records[2].ResourceRecords[0]);
            Assert.AreEqual("ns-368.awsdns-46.com.", records[2].ResourceRecords[1]);
            Assert.AreEqual("ns-1061.awsdns-04.org.", records[2].ResourceRecords[2]);
            Assert.AreEqual("ns-2022.awsdns-60.co.uk.", records[2].ResourceRecords[3]);
        }
    }
}
