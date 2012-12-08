using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginMetadataFactory : MarshalByRefObject, IPluginMetadataFactory
    {
        private readonly IKernel _kernel;

        public PluginMetadataFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginMetadata CreateMetadata(Type interfaceType, 
            string name, string description, double version, string author, string identifier, Type controllerType)
        {
            return _kernel.Get<IPluginMetadata>(new[]
            {
                new ConstructorArgument("interfaceType", interfaceType),
                new ConstructorArgument("name", name),
                new ConstructorArgument("description", description),
                new ConstructorArgument("version", version),
                new ConstructorArgument("author", author),
                new ConstructorArgument("identifier", identifier),
                new ConstructorArgument("controllerType", controllerType),
            });
        }

        public IPluginMetadata CreateMetadataFromPluginAttribute(PluginAttribute attribute)
        {
            return CreateMetadata(
                attribute.InterfaceType,
                attribute.Name,
                attribute.Description,
                attribute.Version,
                attribute.Author,
                attribute.Identifier,
                attribute.ControllerType);
        }
    }
}
