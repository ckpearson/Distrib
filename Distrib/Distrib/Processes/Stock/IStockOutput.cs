using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes.Stock
{
    public interface IStockOutput<T>
    {
        T Output { get; set; }
    }

    public interface IStockOutput<T1, T2>
    {
        T1 FirstOutput { get; set; }
        T2 SecondOutput { get; set; }
    }
}
