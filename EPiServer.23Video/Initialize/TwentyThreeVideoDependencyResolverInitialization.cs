using EPiCode.TwentyThreeVideo.Provider;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using StructureMap;

namespace EPiCode.TwentyThreeVideo.Initialize
{
    [InitializableModule]
    public class TwentyThreeVideoDependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }

        private static void ConfigureContainer(ConfigurationExpression container)
        {
            container.For<SettingsRepository>().Use<SettingsRepository>();
        }

        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
        }
    }
}