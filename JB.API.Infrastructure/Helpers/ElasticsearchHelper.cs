﻿using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Helpers
{
    public static class ElasticsearchHelper
    {
        public static BoolQuery GetContainQuery<T>(string field, T[] values)
            => string.IsNullOrEmpty(field) || values == null || values.Length == 0
                ? null
                : new()
                {
                    Should = values.Select(x => new QueryContainer(new TermQuery
                    {
                        Field = field,
                        Value = x switch
                        {
                            string s => s.ToLower(),
                            _ => x,
                        },
                    }))
                };
        public static BoolQuery AppendToMustQuery(this BoolQuery boolQuery, QueryBase childBoolQuery)
        {
            if (childBoolQuery != null)
            {
                boolQuery.Must = boolQuery.Must
                            .Append(
                                new QueryContainer(
                                    childBoolQuery
                                )
                            );
            }

            return boolQuery;
        }
    }
}
