using Ludo.Core.Pools.Runtime;
using Ludo.UnityInject;
using UnityEngine;


namespace Ludo.Demo.Installers
{
    [CreateAssetMenu(fileName = "CoreManagersInstaller", menuName = "Ludo Demo/Installers/CoreManagersInstaller")]
    public class CoreManagersInstaller : ScriptableObjectInstaller
    {
        [Header("Core Managers")]
        [SerializeField] private PoolManager poolManagerPrefab;
        
        public override void InstallBindings(IContainer container)
        {
            BindAndLog<IPoolManager, PoolManager>(container, poolManagerPrefab, "PoolManager");
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