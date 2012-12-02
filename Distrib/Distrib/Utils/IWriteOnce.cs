using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    public interface IWriteOnce<T>
    {
        bool IsWritten { get; }
        T Value { get; set; }
    }
}
