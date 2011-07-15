using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Amazon.Domain
{
    /// <summary>
    /// Represents a single Resource Record Set from the Amazon Route 53 DNS service
    /// </summary>
    public class ResourceRecordSet : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRecordSet"/> class.
        /// </summary>
        public ResourceRecordSet()
        {
            ResourceRecords = new List<string>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ResourceRecordSetType Type { get; set; }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        public int TTL { get; set; }

        /// <summary>
        /// Gets or sets the resource records.
        /// </summary>
        /// <value>
        /// The resource records.
        /// </value>
        public IList<string> ResourceRecords { get; private set; }

        /// <summary>
        /// Adds the resource records.
        /// </summary>
        /// <param name="records">The records.</param>
        public void AddResourceRecords(IEnumerable<string> records)
        {
            foreach (var record in records)
            {
                ResourceRecords.Add(record);
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new ResourceRecordSet { Name = Name, TTL = TTL, Type = Type };

            clone.AddResourceRecords(ResourceRecords);

            return clone;
        }

        public XElement ToChangeRequest(string action)
        {
            XNamespace ns = "https://route53.amazonaws.com/doc/2011-05-05/";

            return new XElement(ns + "Change",
                            new XElement(ns + "Action", action),
                            new XElement(ns + "ResourceRecordSet",
                            new XElement(ns + "Name", Name + "."),
                            new XElement(ns + "Type", Type.ToString()),
                            new XElement(ns + "TTL", TTL),
                            new XElement(ns + "ResourceRecords",
                            new XElement(ns + "ResourceRecord", 
                                new XElement(ns + "Value", ResourceRecords[0])
                    ))));
        }
    }
}
