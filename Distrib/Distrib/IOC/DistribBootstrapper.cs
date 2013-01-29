/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public abstract class DistribBootstrapper : CrossAppDomainObject, IIOC
    {
        protected virtual void Initialise() { }
        protected abstract object GetInstance(Type serviceType, params IOCConstructorArgument[] args);
        protected abstract IEnumerable<object> GetAllInstances(Type serviceType);
        protected abstract void Inject(object instance);

        protected abstract void Bind(Type serviceType, Type concreteType, bool singleton = false);
        protected abstract void BindToConstant(Type serviceType, object instance);

        protected abstract bool IsTypeRegistered(Type serviceType);

        private static DistribBootstrapper _currentBootstrapper = null;
        private static object _lock = new object();
        internal static DistribBootstrapper CurrentBoostrapper
        {
            get
            {
                lock (_lock)
                {
                    return _currentBootstrapper;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentBootstrapper = value;
                }
            }
        }

        public void Start()
        {
            try
            {
                if (DistribBootstrapper.CurrentBoostrapper != null)
                {
                    throw new InvalidOperationException("A boostrapper has already been started");
                }
                else
                {
                    DistribBootstrapper.CurrentBoostrapper = this;
                }

                this.Initialise();

                // Ensure that Distrib can access the IOC goodness
                this.BindToConstant(typeof(IIOC), this);

                // Now go out and ask all the IOC registrar classes to chime in
                var registrars = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => (t.BaseType != null && t.BaseType.Equals(typeof(IOCRegistrar))) && t.GetConstructor(Type.EmptyTypes) != null)
                    .Select(t => (IOCRegistrar)Activator.CreateInstance(t));

                foreach (var registrar in registrars)
                {
                    registrar.PerformBindings();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to start bootstrapping for Distrib", ex);
            }
        }

        internal void DoBind(Type serviceType, Type concreteType, bool singleton = false)
        {
            this.Bind(serviceType, concreteType, singleton);
        }

        public T Get<T>(params IOCConstructorArgument[] args)
        {
            return (T)this.GetInstance(typeof(T), args);
        }


        public object Get(Type serviceType, params IOCConstructorArgument[] args)
        {
            return this.GetInstance(serviceType, args);
        }


        public bool IsTypeBound<T>()
        {
            return this.IsTypeRegistered(typeof(T));
        }

        public bool IsTypeBound(Type serviceType)
        {
            return this.IsTypeRegistered(serviceType);
        }
    }

    public sealed class IOCConstructorArgument
    {
        private readonly string _argName;
        private readonly object _value;

        public IOCConstructorArgument(string name, object value)
        {
            _argName = name;
            _value = value;
        }

        public string Name { get { return _argName; } }
        public object Value { get { return _value; } }
    }

    public interface IIOC
    {
        T Get<T>(params IOCConstructorArgument[] args);
        object Get(Type serviceType, params IOCConstructorArgument[] args);
        bool IsTypeBound<T>();
        bool IsTypeBound(Type serviceType);
        void Start();
    }

    public abstract class IOCRegistrar
    {
        public abstract void PerformBindings();

        protected void Bind(Type serviceType, Type concreteType)
        {
            Ex.ArgNull(() => serviceType);
            Ex.ArgNull(() => concreteType);

            try
            {
                if (DistribBootstrapper.CurrentBoostrapper == null)
                {
                    throw new InvalidOperationException("No bootstrapper has been started");
                }

                DistribBootstrapper.CurrentBoostrapper.DoBind(serviceType, concreteType, false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to perform binding", ex);
            }
        }

        protected void Bind<TFrom, TTO>() where TFrom : class where TTO: class
        {
            Bind(typeof(TFrom), typeof(TTO));
        }

        protected void BindSingleton(Type serviceType, Type concreteType)
        {
            Ex.ArgNull(() => serviceType);
            Ex.ArgNull(() => concreteType);

            try
            {
                if (DistribBootstrapper.CurrentBoostrapper == null)
                {
                    throw new InvalidOperationException("No bootstrapper has been started");
                }

                DistribBootstrapper.CurrentBoostrapper.DoBind(serviceType, concreteType, true);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to perform binding for singleton", ex);
            }
        }

        protected void BindSingleton<TFrom, TTO>() where TFrom : class where TTO: class
        {
            BindSingleton(typeof(TFrom), typeof(TTO));
        }
    }
}
