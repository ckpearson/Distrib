using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    internal static class StringUtils
    {
        public static string fmt(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}
