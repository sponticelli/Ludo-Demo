using Ludo.AudioFlux;
using Ludo.UnityInject;
using UnityEngine;

namespace Bloomblat.Installers
{
    [CreateAssetMenu(fileName = "AudioFluxInstaller", menuName = "Bloomblat/Installers/AudioFluxInstaller")]
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