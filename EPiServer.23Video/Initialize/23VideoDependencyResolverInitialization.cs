using EPiServer.Framework;
using EPiServer.ServiceLocation;
using StructureMap;

namespace EPiServer._23Video.Initialize
{
    [InitializableModule]
    public class _23VideoDependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }

        private static void ConfigureContainer(ConfigurationExpression container)
        {
            container.For<_23VideoSettingsRepository>().Use<_23VideoSettingsRepository>();
        }

        public void Initialize(Framework.Initialization.InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(Framework.Initialization.InitializationEngine context)
        {
        }
    }
}