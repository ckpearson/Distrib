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

    public sealed class StockOutput<T> : IStockOutput<T>
    {
        private readonly IJob _job;
        private readonly object _lock = new object();

        public StockOutput(IJob job)
        {
            if (job == null) throw Ex.ArgNull(() => job);

            _job = job;
        }

        public T Output
        {
            get
            {
                lock (_lock)
                {
                    return _job.OutputTracker.GetOutput<T>(_job);
                }
            }
            set
            {
                lock (_lock)
                {
                    _job.OutputTracker.SetOutput<T>(_job, value);
                }
            }
        }
    }
}
