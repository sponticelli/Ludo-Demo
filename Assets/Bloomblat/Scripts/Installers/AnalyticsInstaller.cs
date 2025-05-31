using Ludo.Core.Analytics;
using Ludo.UnityInject;
using UnityEngine;

namespace Ludo.Demo.Installers
{
    [CreateAssetMenu(fileName = "AnalyticsInstaller", menuName = "Ludo Demo/Installers/AnalyticsInstaller")]
    public class AnalyticsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private DebugProviderConfig debugProviderConfig;

        public override void InstallBindings(IContainer container)
        {
            // Bind the analytics service as a singleton
            container.Bind<IAnalyticsService>().To<AnalyticsService>().AsSingleton();

            // Bind the debug provider config
            container.Bind<DebugProviderConfig>().FromInstance(debugProviderConfig);
        }
    }
}