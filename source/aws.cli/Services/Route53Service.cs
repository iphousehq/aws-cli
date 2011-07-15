using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Amazon.Domain;
using Amazon.Mappers;
using Comsec.Sugar;
using Comsec.Sugar.Net;

namespace Amazon.Services
{
    /// <summary>
    /// Encapsulates the Amazon Route 53 DNS service.
    /// </summary>
    public class Route53Service
    {
        /// <summary>
        /// Gets or sets the HTTP service.
        /// </summary>
        /// <value>
        /// The HTTP service.
        /// </value>
        public IHttpService HttpService { get; set; }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route53Service"/> class.
        /// </summary>
        public Route53Service()
        {
            HttpService = new HttpService();
            Credentials = new Credentials();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route53Service"/> class.
        /// </summary>
        /// <param name="httpService">The HTTP service.</param>
        /// <param name="credentials">The credentials.</param>
        public Route53Service(IHttpService httpService, ICredentials credentials)
        {
            HttpService = httpService;
            Credentials = credentials;
        }

        public HostedZoneDescriptor GetZone(string domain)
        {
            return ListHostedZones().FirstOrDefault(z => string.Compare(z.Name, domain, true) == 0);
        }

        public IList<HostedZoneDescriptor> ListHostedZones()
        {
            var results = new List<HostedZoneDescriptor>();

            var request = new HttpRequest { Url = "https://route53.amazonaws.com/2011-05-05/hostedzone" };

            SetAuthentication(request);

            var response = HttpService.Get(request);

            if (response.Success)
            {
                results.AddRange(new HostedZoneDescriptorMapper().Map(response.ToString()));
            }

            return results;
        }

        private void SetAuthentication(HttpRequest request)
        {
            // the canonical string is the date string 
            string httpDate = GetRoute53Date();

            request.Headers.Add("x-amz-date", httpDate);

            // Both the following methods work! 
            string authenticationSig = GetAwsr53Sha1AuthorizationValue(httpDate);

            request.Headers.Add("X-Amzn-Authorization", authenticationSig);
        }

        /// <summary>
        /// Gets the hosted zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <returns></returns>
        public HostedZone GetHostedZone(string zoneId)
        {
            HostedZone zone = null;

            var request = new HttpRequest { Url = "https://route53.amazonaws.com/2011-05-05/hostedzone/" + zoneId };

            SetAuthentication(request);

            var response = HttpService.Get(request);

            if (response.Success)
            {
                zone = new HostedZoneMapper().Map(response.ToString());
            }

            return zone;
        }

        public IList<ResourceRecordSet> ListResourceRecordSets(string zoneId)
        {
            var results = new List<ResourceRecordSet>();

            var request = new HttpRequest { Url = "https://route53.amazonaws.com/2011-05-05/hostedzone/" + zoneId + "/rrset?maxitems=100", Timeout = 60000 };

            SetAuthentication(request);

            var response = HttpService.Get(request);

            if (response.Success)
            {
                results.AddRange(new ResourceRecordSetMapper().Map(response.ToString()));
            }

            return results;
        }

        public void ChangeResourceRecordSet(string zoneId, ResourceRecordSet original, ResourceRecordSet change)
        {
            XNamespace ns = "https://route53.amazonaws.com/doc/2011-05-05/";

            var doc = new XElement(ns + "ChangeResourceRecordSetsRequest",
                                   new XElement(ns + "ChangeBatch",
                                                new XElement(ns + "Changes", 
                                                    original.ToChangeRequest("DELETE"),
                                                    change.ToChangeRequest("CREATE")
                                                    )));

            var xml = doc.ToString();

            var request = new HttpRequest();
            request.Data = xml;
            request.Url = "https://route53.amazonaws.com/2011-05-05/hostedzone/" + zoneId + "/rrset";

            SetAuthentication(request);

            var response = HttpService.Post(request);

            if (!response.Success)
            {
                Console.WriteLine(response.Exception.Message);
            }
        }

        public string GetPublicIpAddress()
        {
            var html = HttpService.Get("http://checkip.dyndns.org/");

            if (html.Success)
            {
                return html.ToString().Keep("1234567890.");
            }

            throw new ApplicationException("Unable to determine public IP address");
        }

        public string GetAwsr53Sha1AuthorizationValue(string date)
        {
            var signer = new System.Security.Cryptography.HMACSHA1(System.Text.Encoding.UTF8.GetBytes(Credentials.SecretAccessKey));

            string value = Convert.ToBase64String(signer.ComputeHash(System.Text.Encoding.UTF8.GetBytes(date)));

            return "AWS3-HTTPS AWSAccessKeyId=" + Uri.EscapeDataString(Credentials.AccessKeyId) + ",Algorithm=HmacSHA1,Signature=" + value;
        }

        private static string date;

        public static string GetRoute53Date()
        {
            if (string.IsNullOrEmpty(date))
            {
                string url = "https://route53.amazonaws.com/date";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                HttpWebResponse response;
                response = request.GetResponse() as HttpWebResponse;
                date = response.Headers["Date"];
            }

            return date;
        }
    }
}
