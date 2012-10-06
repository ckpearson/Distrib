using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Core interface for defining a Distrib-enabled process
    /// </summary>
    public interface IDistribProcess
    {

    }

    [DistribProcess(Guid = "{44B4644E-9ADF-4940-896C-C5BAC953EB69}",
        Version = 1.0,
        Author = "Clint Pearson",
        Name = "Hello World")]
    public class HelloWorldProcess : IDistribProcess
    {

    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DistribProcessAttribute : ExportAttribute
    {
        public DistribProcessAttribute()
            : base(typeof(IDistribProcess)) { }

        public string Guid { get; set; }
        public double Version { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
    }

    public interface IDistribProcessMetadata
    {
        string Guid { get; }
        double Version { get; }
        string Name { get; }
        string Author { get; }
    }
}
