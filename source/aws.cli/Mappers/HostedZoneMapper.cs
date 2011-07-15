using Amazon.Domain;
using Comsec.Sugar;
using Comsec.Sugar.Xml;

namespace Amazon.Mappers
{
    public class HostedZoneMapper
    {
        public HostedZone Map(string xml)
        {
            var xPath = xml.ToXPath();

            var zone = new HostedZone
                            {
                                Id = xPath.GetInnerXml("//Id").SubstringAfterLastChar("/"),
                                Name = xPath.GetInnerXml("//Name").SubstringBeforeLastChar("."),
                                CallerReference = xPath.GetInnerXml("//CallerReference")
                            };

            var matches = xPath.GetInnerXmlList("//NameServer");

            foreach (var match in matches)
            {
                zone.Nameservers.Add(match);
            }

            return zone;
        }
    }
}
