using ProcessRunner.Models;
using ProcessRunner.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.ViewModels
{
    public interface IPrimaryViewModel
    {
        void Navigate(IView view);
        void DoAsBusy(Action act, Action actFinished = null);
        bool IsBusy { get; }

        PluginAssemblyModel CurrentPluginAssembly { get; }
        void LoadAssembly(string path);
        void InitAssembly();
        void UninitAssembly();
        void UnloadAssembly();
    }
}
