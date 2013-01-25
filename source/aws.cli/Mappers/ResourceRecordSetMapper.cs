using System;
using System.Collections.Generic;
using Amazon.Domain;
using Sugar;
using Sugar.Xml;

namespace Amazon.Mappers
{
    public class ResourceRecordSetMapper
    {
        public IList<ResourceRecordSet> Map(string xml)
        {
            var records = new List<ResourceRecordSet>();

            var xPath = xml.ToXPath();

            var matches = xPath.GetMatches("//ResourceRecordSet");

            foreach (var match in matches)
            {
                var record = new ResourceRecordSet
                                 {
                                     Name = match.GetInnerXml("//Name").SubstringBeforeLastChar("."),
                                     Type = (ResourceRecordSetType) Enum.Parse(typeof (ResourceRecordSetType), match.GetInnerXml("//Type")),
                                     TTL = match.GetInnerXml("//TTL").AsInt32(),
                                 };

                var dnsName = match.GetInnerXml("//DNSName");

                if (!string.IsNullOrEmpty(dnsName) && record.Type == ResourceRecordSetType.A)
                {
                    record.Type = ResourceRecordSetType.AAAA;
                    record.ResourceRecords.Add(dnsName);
                }

                record.AddResourceRecords(match.GetInnerXmlList("//Value"));

                records.Add(record);
            }

            return records;
        }
    }
}
