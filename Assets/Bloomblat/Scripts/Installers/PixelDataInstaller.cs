using Bloomblat.Core;
using Ludo.UnityInject;
using UnityEngine;

namespace Bloomblat.Installers
{
    /// <summary>
    /// Installer for pixel-perfect rendering data provider.
    /// Binds the PixelDataProvider as a singleton for dependency injection.
    /// </summary>
    [CreateAssetMenu(fileName = "PixelDataInstaller", menuName = "Bloomblat/Installers/PixelDataInstaller")]
    public class PixelDataInstaller : ScriptableObjectInstaller
    {
        [Header("Pixel Data Provider")]
        [SerializeField] private PixelDataProvider pixelDataProviderPrefab;

        public override void InstallBindings(IContainer container)
        {
            // Bind PixelDataProvider as a singleton
            container.Bind<IPixelDataProvider>().FromInstance(pixelDataProviderPrefab).AsSingleton();
        }
    }
}