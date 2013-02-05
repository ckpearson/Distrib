using Distrib.Processes;
using Distrib.Processes.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathOperationsProcessLibary
{
    public static class MathOpsProcess_JobDefinitions
    {
        private static IReadOnlyList<IJobDefinition> _definitions;

        public static IReadOnlyList<IJobDefinition> Definitions
        {
            get
            {
                if (_definitions == null)
                {
                    _definitions = new List<IJobDefinition>()
                    {
                        MathOpsProcess_JobDefinitions.AddIntDef,
                    }.AsReadOnly();
                }

                return _definitions;
            }
        }

        private static IJobDefinition<IStockInput<int, int>, IStockOutput<int>> _addIntDef;
        public static IJobDefinition AddIntDef
        {
            get
            {
                if (_addIntDef == null)
                {
                    _addIntDef = new ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>>(
                        "Add Integers",
                        "Adds two integers");
                }

                return _addIntDef;
            }
        }
    }
}
