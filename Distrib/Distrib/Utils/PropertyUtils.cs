/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
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
