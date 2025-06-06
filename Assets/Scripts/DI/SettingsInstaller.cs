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

        [SerializeField]
        EffectsSettings effectsSettings;

        [SerializeField]
        GameWorldSettings gameWorldSettings;

        [SerializeField]
        EnemySettings enemySettings;

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

            Container.Bind<EffectsSettings>()
                .FromInstance(effectsSettings)
                .AsSingle()
                .NonLazy();

            Container.Bind<GameWorldSettings>()
                .FromInstance(gameWorldSettings)
                .AsSingle()
                .NonLazy();

            Container.Bind<EnemySettings>()
                .FromInstance(enemySettings)
                .AsSingle()
                .NonLazy();

        }
    }
}