using Ludo.AudioFlux;
using Ludo.UnityInject;
using UnityEngine;

namespace Ludo.Demo.Installers
{
    [CreateAssetMenu(fileName = "AudioFluxInstaller", menuName = "Ludo Demo/Installers/AudioFluxInstaller")]
    public class AudioFluxInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private SfxService sfxServicePrefab;
        [SerializeField] private MusicService musicServicePrefab;

        public override void InstallBindings(IContainer container)
        {
           BindPersistentComponent<ISFXService, SfxService>(container, sfxServicePrefab);
           BindPersistentComponent<IMusicService, MusicService>(container, musicServicePrefab);
        }
    }
}