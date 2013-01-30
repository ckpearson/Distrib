using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Services
{
    public interface IAppStateService
    {
        bool AppBusy { get; }
        void DoAsBusy(Action act, Action actFinished = null);

        void PerformVisibleTask(Action<Action<string>> taskAction,
            Func<string> finishedAction = null);

        string StatusText { get; }
    }
}
