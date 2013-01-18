using Distrib.Separation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distrib.Utils;
using System.Runtime.CompilerServices;

namespace Distrib.Processes
{
    /// <summary>
    /// Core interface for defining a Distrib-enabled process
    /// </summary>
    public interface IProcess
    {
        void InitProcess();
        void UninitProcess();
        IJobDefinition JobDefinition { get; }

        void ProcessJob(IJob job);
    }
}
