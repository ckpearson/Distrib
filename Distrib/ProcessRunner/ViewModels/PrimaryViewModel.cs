using ProcessRunner.Models;
using ProcessRunner.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProcessRunner.ViewModels
{
    public sealed class PrimaryViewModel : ViewModelBase, IPrimaryViewModel
    {
        public bool IsBusy
        {
            get
            {
                return Interlocked.Read(ref _busyTasks) > 0;
            }
        }

        private IView _currentView;
        public IView CurrentView
        {
            get
            {
                return _currentView;
            }

            private set
            {
                _currentView = value;
                onPropChange();
            }
        }

        private long _busyTasks = 0;

        private void AddBusy()
        {
            Interlocked.Increment(ref _busyTasks);
            onPropChange("IsBusy");
        }

        private void TakeBusy()
        {
            Interlocked.Decrement(ref _busyTasks);
            onPropChange("IsBusy");
        }

        private PluginAssemblyModel _currentAssembly;
        public PluginAssemblyModel CurrentPluginAssembly
        {
            get
            {
                return _currentAssembly;
            }
            private set
            {
                _currentAssembly = value;
                onPropChange();
            }
        }

        public void DoAsBusy(Action act, Action actFinished = null)
        {
            AddBusy();
            Task.Factory.StartNew(act)
                .ContinueWith(t =>
                    {
                        TakeBusy();
                        if (actFinished != null)
                        {
                            actFinished();
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Navigate(IView view)
        {
            DoAsBusy(() =>
                {
                    IView prevView = CurrentView;
                    if (prevView != null)
                    {
                        prevView.NavigatingFrom();
                    }

                    view.NavigatingTo();

                    CurrentView = view;

                    if (prevView != null)
                    {
                        prevView.NavigatedFrom();
                    }

                    CurrentView.NavigatedTo();
                });
        }


        public void LoadAssembly(string path)
        {
            if (CurrentPluginAssembly != null)
            {
                if (CurrentPluginAssembly.IsInitialised)
                {
                    CurrentPluginAssembly.Uninitialise();
                }

                CurrentPluginAssembly = null;
            }

            CurrentPluginAssembly = new PluginAssemblyModel(path);
        }

        public void InitAssembly()
        {
            if (CurrentPluginAssembly == null)
            {
                throw new InvalidOperationException("Cannot initialise assembly, not assembly has been set yet");
            }

            CurrentPluginAssembly.Initialise();
        }

        public void UninitAssembly()
        {
            if (CurrentPluginAssembly == null)
            {
                throw new InvalidOperationException();
            }

            if (!CurrentPluginAssembly.IsInitialised)
            {
                throw new InvalidOperationException();
            }

            CurrentPluginAssembly.Uninitialise();
        }

        public void UnloadAssembly()
        {
            if (CurrentPluginAssembly == null)
            {
                throw new InvalidOperationException();
            }

            if (CurrentPluginAssembly.IsInitialised)
            {
                CurrentPluginAssembly.Uninitialise();
            }

            CurrentPluginAssembly = null;
        }
    }
}
