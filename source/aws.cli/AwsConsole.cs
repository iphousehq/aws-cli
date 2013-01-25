using System;
using Amazon.Commands;
using Sugar.Command;

namespace Amazon
{
    /// <summary>
    /// Amazon Web Service Console
    /// </summary>
    public class AwsConsole : BaseCommandConsole
    {
        public AwsConsole()
        {
            Commands.Add(new ListHostedZones());
            Commands.Add(new GetHostedZone());
            Commands.Add(new ListResourceRecordSets());
            Commands.Add(new ChangeResourceRecordSets());
        }

        public override int Default()
        {
            return 0;
        }
    }
}
