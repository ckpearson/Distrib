using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes.Stock
{
    public interface IStockInput<T>
    {
        T Input { get; }
    }

    public interface IStockInput<T1, T2>
    {
        T1 FirstInput { get; }
        T2 SecondInput { get; }
    }
}
