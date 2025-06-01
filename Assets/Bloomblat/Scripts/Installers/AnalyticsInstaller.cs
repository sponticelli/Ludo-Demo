using Ludo.Core.Analytics;
using Ludo.UnityInject;
using UnityEngine;

namespace Bloomblat.Installers
{
    [CreateAssetMenu(fileName = "AnalyticsInstaller", menuName = "Bloomblat/Installers/AnalyticsInstaller")]
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