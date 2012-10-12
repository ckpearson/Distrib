using Distrib.Processes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        
        public void Run()
        {
            var dir = @"C:\Users\Clint\Desktop\distrib plugins\";

            foreach (var plugindll in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var ad = AppDomain.CreateDomain(plugindll);
                var asmManager = ad.CreateInstanceAndUnwrap(typeof(AppDomainAssemblyManager).Assembly.FullName,
                    typeof(AppDomainAssemblyManager).FullName) as AppDomainAssemblyManager;
                asmManager.LoadAssembly(plugindll);
                IDistribProcess proc = asmManager.CreateProcessTypeFromAssembly(plugindll,
                    "TestLibrary.TestDistribProcess");
                var s = proc.SayHello();
                AppDomain.Unload(ad);
                File.Delete(plugindll);
            }
        }
    }

    public sealed class IDistribProcessProxy : MarshalByRefObject, IDistribProcess
    {
        private IDistribProcess m_masked = null;

        public IDistribProcessProxy(IDistribProcess maskedProcess)
        {
            m_masked = maskedProcess;
        }

        public string SayHello()
        {
            return m_masked.SayHello();
        }
    }

    public sealed class AppDomainAssemblyManager : MarshalByRefObject
    {
        private ConcurrentDictionary<string, Assembly> m_dictAssemblies =
            new ConcurrentDictionary<string, Assembly>();

        public void LoadAssembly(string filePath)
        {
            try
            {
                var asm = Assembly.LoadFile(filePath);
                if (m_dictAssemblies.TryAdd(filePath, asm) == false)
                {
                    throw new ApplicationException("Failed to add assembly to store");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load assembly", ex);
            }
        }

        public IDistribProcess CreateProcessTypeFromAssembly(string filePath, string typeName)
        {
            try
            {
                Assembly asm = null;
                if (m_dictAssemblies.TryGetValue(filePath, out asm) == false)
                {
                    throw new ApplicationException("Failed to find assembly in store");
                }

                var obj = asm.CreateInstance(typeName) as IDistribProcess;
                return new IDistribProcessProxy(obj);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get process type from assembly", ex);
            }
        }

        public void RemoveAssembly(string filePath)
        {
            try
            {
                Assembly asm = null;
                if (m_dictAssemblies.TryRemove(filePath, out asm) == false)
                {
                    throw new ApplicationException("Failed to remove assembly from store");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to remove assembly", ex);
            }
        }
    }
}
