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

    public sealed class StockInput<T> : IStockInput<T>
    {
        private readonly IJob _job;
        private readonly object _lock = new object();

        public StockInput(IJob job)
        {
            if (job == null) throw Ex.ArgNull(() => job);

            _job = job;
        }

        public T Input
        {
            get
            {
                lock (_lock)
                {
                    return _job.InputTracker.GetInput<T>(_job); 
                }
            }
        }
    }

    public sealed class StockInput<T1, T2> : IStockInput<T1, T2>
    {
        private readonly IJob _job;
        private readonly object _lock = new object();

        public StockInput(IJob job)
        {
            if (job == null) throw Ex.ArgNull(() => job);

            _job = job;
        }

        public T1 FirstInput
        {
            get
            {
                lock (_lock)
                {
                    return _job.InputTracker.GetInput<T1>(_job); 
                }
            }
        }

        public T2 SecondInput
        {
            get
            {
                lock (_lock)
                {
                    return _job.InputTracker.GetInput<T2>(_job); 
                }
            }
        }
    }
}
