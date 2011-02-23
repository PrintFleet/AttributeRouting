using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AttributeRouting.Extensions
{
    public static class ObjectExtensions
    {
        public static TValue SafeGet<T, TValue>(this T obj, Expression<Func<T, TValue>> expression)
            where T : class
            where TValue : class
        {
            if (obj == null)
                return null;

            return expression.Compile().Invoke(obj);
        }
    }
}
