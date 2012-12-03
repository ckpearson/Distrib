using Distrib.IOC.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC.Interface
{
    /// <summary>
    /// When derived provides a means of offering an IOC-agnostic lazy singleton registration module
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IOCReg<T> where T : IIOCRegistrationModule
    {
        private static IIOCRegistrationModule _module;

        /// <summary>
        /// Gets the IOC registration module
        /// </summary>
        public static IIOCRegistrationModule Module
        {
            get
            {
                if (_module == null)
                    _module = (IIOCRegistrationModule)Activator.CreateInstance<T>();

                return _module;
            }
        }

        /// <summary>
        /// Helper method to perform invocation of the bind action and handle exceptions
        /// </summary>
        /// <typeparam name="TFrom">The type to bind from</typeparam>
        /// <typeparam name="TTo">The type to bind to</typeparam>
        /// <param name="bindAction"></param>
        protected void Bind<TFrom, TTo>(Action<Type, Type> bindAction)
            where TFrom : class
            where TTo : class
        {
            if (bindAction == null) throw new ArgumentNullException("Bind action not supplied");

            try
            {
                bindAction(typeof(TFrom), typeof(TTo));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format("Failed to perform binding betwen type '{0}' and '{1}'.",
                        typeof(TFrom).FullName,
                        typeof(TTo).FullName), ex);
                        
            }
        }
    }
}
