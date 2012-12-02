using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    public static class ObjectUtils
    {
        public static T NullIfSo<T>(this T value) where T : class
        {
            return value == null ? null : value;
        }
    }
}
