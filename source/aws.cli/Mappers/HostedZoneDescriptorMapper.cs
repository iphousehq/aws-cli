using System.Collections.Generic;
using Amazon.Domain;
using Comsec.Sugar;
using Comsec.Sugar.Xml;

namespace Amazon.Mappers
{
    public class HostedZoneDescriptorMapper
    {
        public IList<HostedZoneDescriptor> Map(string xml)
        {
            var results = new List<HostedZoneDescriptor>();

            var xPath = xml.ToXPath();

            var nodes = xPath.GetMatches("//HostedZone");

            foreach (var node in nodes)
            {
                var zone = new HostedZoneDescriptor
                               {
                                   Id = node.GetInnerXml("//Id").SubstringAfterLastChar("/"),
                                   Name = node.GetInnerXml("//Name").SubstringBeforeLastChar("."),
                                   CallerReference = node.GetInnerXml("//CallerReference")
                               };

                results.Add(zone);
            }

            return results;
        }
    }
}
