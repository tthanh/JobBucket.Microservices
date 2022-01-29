using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.API.Gateway.GraphQL.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class SampleQuery
    {
        public string Index => "JB.API";
    }
}
