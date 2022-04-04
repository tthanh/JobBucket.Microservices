using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Helpers
{
    public static class ElasticsearchHelper
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
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
                            string s => s.RemoveSpecialCharacters().ToLower(),
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

        public static BoolQuery AppendToShouldQuery(this BoolQuery boolQuery, QueryBase childBoolQuery)
        {
            if (childBoolQuery != null)
            {
                boolQuery.Should = boolQuery.Should
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
