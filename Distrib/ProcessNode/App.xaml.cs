using DistribApps.Core.ViewModels;
using Microsoft.Practices.Prism.MefExtensions;
using ProcessNode.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessNode
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var prismboot = new PrismBootstrapper();
            prismboot.Run();
        }
    }

    public sealed class PrismBootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(PrismBootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ViewModelBase).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AppRegions).Assembly));
            string appPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string extendPath = System.IO.Path.Combine(appPath, "Extend");
            this.AggregateCatalog.Catalogs.Add(new DirectoryCatalog(extendPath));
        }

        protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            return base.ConfigureDefaultRegionBehaviors();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.GetExport<MainWindow>().Value;
        }

        protected override void InitializeModules()
        {
            try
            {
                base.InitializeModules();
            }
            catch (CompositionException cex)
            {
                var errs = cex.Errors;
                throw cex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }
    }

}
