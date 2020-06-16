using System.Threading.Tasks;
using Comsec.Aws.Services;
using LightInject;
using Moq;
using NUnit.Framework;

namespace Comsec.Aws.Commands
{
    [TestFixture]
    [Parallelizable]
    public class ListResourceRecordSetsTest
    {
        public ListResourceRecordSetsCommand Setup(out MockingServiceContainer context)
        {
            context = new MockingServiceContainer();

            context.Register<ListResourceRecordSetsCommand>();

            return context.GetInstance<ListResourceRecordSetsCommand>();
        }

        [Test]
        public async Task TestExecuteWhenNoRecordsForZoneId()
        {
            var command = Setup(out var context);

            var input = new ListResourceRecordSetsCommand.Input("AABBDDCCDDEE");

            await command.Execute(input);

            context.GetMock<IRoute53Service>()
                   .Verify(call => call.ListResourceRecordSets("AABBDDCCDDEE"), Times.Once());
        }
    }
}
