using Ludo.Core.Analytics;
using Ludo.Core.Pools.Runtime;
using Ludo.UnityInject;
using UnityEngine;
using USpring;


namespace Ludo.Demo.Installers
{
    [CreateAssetMenu(fileName = "CoreManagersInstaller", menuName = "Ludo Demo/Installers/CoreManagersInstaller")]
    public class CoreManagersInstaller : ScriptableObjectInstaller
    {
        [Header("Core Managers")]
        [SerializeField] private PoolManager poolManagerPrefab;
        [SerializeField] private SpringSettingsProvider springSettingsProviderPrefab;
        
        public override void InstallBindings(IContainer container)
        {
            container.Bind<IAnalyticsService>().To<AnalyticsService>().AsSingleton();
            BindAndLog<IPoolManager, PoolManager>(container, poolManagerPrefab, "PoolManager");
            BindAndLog<ISpringSettingsProvider, SpringSettingsProvider>(container, springSettingsProviderPrefab, "SpringSettingsProvider");
        }
        
        private void BindAndLog<TInterface, TImplementation>(IContainer container, TImplementation prefab, string managerName)
            where TInterface : class 
            where TImplementation : MonoBehaviour, TInterface
        {
            bool bound = BindPersistentComponent<TInterface, TImplementation>(container, prefab);
            if (!bound)
            {
                Debug.LogError($"[CoreManagersInstaller] {managerName} binding process failed!");
            }
        }
    }
}