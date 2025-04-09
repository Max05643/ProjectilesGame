using Projectiles.Settings;
using UnityEngine;
using Zenject;


namespace Projectiles.DI
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField]
        ProjectileSettings projectileSettings;

        [SerializeField]
        CharacterSettings characterSettings;

        public override void InstallBindings()
        {
            Container.Bind<ProjectileSettings>()
                .FromInstance(projectileSettings)
                .AsSingle()
                .NonLazy();

            Container.Bind<CharacterSettings>()
                .FromInstance(characterSettings)
                .AsSingle()
                .NonLazy();
        }
    }
}