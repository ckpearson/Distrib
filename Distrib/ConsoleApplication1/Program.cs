using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        [ImportMany]
        private Lazy<IDistribProcess, IDistribProcessMetadata>[] Processes { get; set; }
        
        public void Run()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IDistribProcess).Assembly));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            if (Processes != null && Processes.Length > 0)
            {
                
            }
        }
    }
}
