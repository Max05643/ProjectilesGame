using Projectiles.Projectiles;
using UnityEngine;
using Zenject;

namespace Projectiles.DI
{
    public class GlobalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<UnityEngine.Object, ProjectileController, ProjectileController.Factory>().FromFactory<PrefabFactory<ProjectileController>>();
        }
    }
}