using Projectiles.Characters;
using Projectiles.Music;
using Projectiles.Physics;
using Projectiles.Projectiles;
using UnityEngine;
using Zenject;

namespace Projectiles.DI
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField]
        GameObject projectilesCreator;

        [SerializeField]
        GameObject musicSource;

        public override void InstallBindings()
        {
            Container.Bind<MusicController>().FromComponentInNewPrefab(musicSource).AsSingle().NonLazy();

            Container.Bind<ProjectilesCreator>().FromComponentInNewPrefab(projectilesCreator).AsSingle().NonLazy();

            Container.BindFactory<UnityEngine.Object, ProjectileController, ProjectileController.Factory>().FromFactory<PrefabFactory<ProjectileController>>();
            Container.BindFactory<UnityEngine.Object, EnemyController, EnemyController.Factory>().FromFactory<PrefabFactory<EnemyController>>();
        }
    }
}