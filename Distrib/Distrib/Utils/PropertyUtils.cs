using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    public static class PropertyUtils
    {
        public static bool HasGetterAndSetter(this PropertyInfo pi)
        {
            return pi.CanRead && pi.CanWrite;
        }

        public static bool IsForPropertyInfo<TType, TProp>(this Expression<Func<TType, TProp>> expr)
        {
            var mem = expr.Body as MemberExpression;
            if (mem == null) return false;
            var prop = mem.Member as PropertyInfo;
            if (prop == null) return false;

            return true;
        }

        public static PropertyInfo GetPropertyInfo<TType, TProp>(this Expression<Func<TType, TProp>> expr)
        {
            if (!expr.IsForPropertyInfo())
            {
                throw new ArgumentException("Expression doesn't point to a property");
            }

            return ((MemberExpression)expr.Body).Member as PropertyInfo;
        }
    }
}
