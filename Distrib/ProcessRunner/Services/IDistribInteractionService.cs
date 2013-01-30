using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Services
{
    public interface IDistribInteractionService
    {
        void LoadAssembly(string assemblyPath);
        void UnloadAssembly();
        Models.PluginAssembly CurrentAssembly { get; }
    }
}
