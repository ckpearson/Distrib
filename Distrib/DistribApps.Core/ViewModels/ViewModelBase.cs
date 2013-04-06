using DistribApps.Core.ViewServices;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DistribApps.Core.ViewModels
{
    public abstract class ViewModelBase :
        INotifyPropertyChanged,
        INavigationAware,
        IActiveAware,
        IRefreshable
    {
        private readonly bool _supportsRefresh;
        private bool _refreshEnabled = false;

        private readonly ReaderWriterLockSlim _refreshLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        protected ViewModelBase(bool supportsRefresh)
        {
            _supportsRefresh = supportsRefresh;
            _refreshEnabled = _supportsRefresh;
        }

        protected ViewModelBase()
            : this(false)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropChanged([CallerMemberName] string property = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        protected virtual bool OnNavigationTargetCheck(NavigationContext context)
        {
            return true;
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return this.OnNavigationTargetCheck(navigationContext);
        }

        protected virtual void OnNavigatedFrom(NavigationContext context)
        {

        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.OnNavigatedFrom(navigationContext);
        }

        protected virtual void OnNavigatedTo(NavigationContext context)
        {
        }

        // Refresh interface needs to not be explicit so it can be bound (?)
        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            this.OnNavigatedTo(navigationContext);
        }

        protected void EnableRefresh()
        {
            if (!_supportsRefresh)
            {
                throw new InvalidOperationException("View doesn't support refreshing");
            }

            bool bDidWrite = false;

            try
            {
                _refreshLock.EnterReadLock();

                if (!_refreshEnabled)
                {
                    _refreshLock.ExitReadLock();

                    _refreshLock.EnterWriteLock();
                    bDidWrite = true;
                    _refreshEnabled = true;
                    PropChanged("CanRefresh");
                    if (this.CanRefreshChanged != null)
                    {
                        this.CanRefreshChanged(this, _refreshEnabled);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to enable refresh", ex);
            }
            finally
            {
                if (bDidWrite)
                {
                    _refreshLock.ExitWriteLock();
                    bDidWrite = false;
                }
                else
                {
                    if (_refreshLock.IsReadLockHeld)
                    {
                        _refreshLock.ExitReadLock(); 
                    }
                }
            }
        }

        protected void DisableRefresh()
        {
            if (!_supportsRefresh)
            {
                throw new InvalidOperationException("View doesn't support refreshing");
            }

            bool bDidWrite = false;

            try
            {
                _refreshLock.EnterReadLock();

                if (_refreshEnabled)
                {
                    _refreshLock.ExitReadLock();

                    _refreshLock.EnterWriteLock();
                    bDidWrite = true;
                    _refreshEnabled = false;
                    PropChanged("CanRefresh");
                    if (this.CanRefreshChanged != null)
                    {
                        this.CanRefreshChanged(this, _refreshEnabled);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to disable refresh", ex);
            }
            finally
            {
                if (bDidWrite)
                {
                    _refreshLock.ExitWriteLock();
                    bDidWrite = false;
                }
                else
                {
                    if (_refreshLock.IsReadLockHeld)
                    {
                        _refreshLock.ExitReadLock();
                    }
                }
            }
        }

        public bool CanRefresh
        {
            get
            {
                try
                {
                    _refreshLock.EnterReadLock();

                    return _supportsRefresh & _refreshEnabled;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (_refreshLock.IsReadLockHeld)
                    {
                        _refreshLock.ExitReadLock();
                    }
                }
            }
        }

        protected virtual void OnViewRefreshRequested()
        {

        }

        public void Refresh()
        {
            if (!_supportsRefresh)
            {
                throw new InvalidOperationException("View doesn't support refreshing");
            }

            try
            {
                if (!_refreshEnabled)
                {
                    throw new ApplicationException("View refreshing isn't currently enabled");
                }

                try
                {
                    this.OnViewRefreshRequested();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Viewmodel implementation threw exception while refreshing", ex);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to refresh view", ex);
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (this.IsActiveChanged != null)
                {
                    this.IsActiveChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler IsActiveChanged;


        public event Action<IRefreshable, bool> CanRefreshChanged;
    }
}
