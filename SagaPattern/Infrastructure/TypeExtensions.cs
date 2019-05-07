using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaPattern.Infrastructure
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> RelatedMessageTypes(this Type type)
        {
            return type
                .GetInterfaces()
                .Concat(type.Ancestors())
                .Concat(new[] { type })
                .Where(typeof(IMessage).IsAssignableFrom)
                .Distinct();
        }

        private static IEnumerable<Type> Ancestors(this Type type)
        {
            var ret = type?.BaseType;

            while (ret != null)
            {
                yield return ret;
                ret = ret.BaseType;
            }
        }
    }
}