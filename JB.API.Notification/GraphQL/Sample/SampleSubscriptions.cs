using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.API.Notification.GraphQL.Sample
{
    [ExtendObjectType(OperationTypeNames.Subscription)]
    public class SampleSubscriptions
    {
        [SubscribeAndResolve]
        [Topic("sample")]
        public ValueTask<ISourceStream<string>> Sample() => new ValueTask<ISourceStream<string>>();
    }
}
