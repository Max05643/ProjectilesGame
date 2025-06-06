using System.Collections;
using System.Collections.Generic;
using Projectiles.Projectiles;
using Projectiles.Settings;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;


namespace Projectiles.Physics
{
    public class ProjectilesCreator : MonoBehaviour
    {
        [SerializeField]
        GameObject projectilePrefabPlayer, projectilePrefabEnemy;

        [Inject]
        ProjectileSettings projectileSettings;

        [Inject]
        ProjectileController.Factory projectileFactory;

        /// <summary>
        /// Creates a projectile object which belongs to player and sets its direction and speed
        /// </summary>
        public void FirePlayersProjectile(Transform original, Vector3 basicForwardDirection, float horizontalProjectileAngle, float verticalProjectileAngle)
        {
            FireProjectile(projectilePrefabPlayer, original, basicForwardDirection, horizontalProjectileAngle, verticalProjectileAngle);
        }

        /// <summary>
        /// Creates a projectile object which belongs to enemy and sets its direction and speed
        /// </summary>
        public void FireEnemiesProjectile(Transform original, Vector3 basicForwardDirection, float horizontalProjectileAngle, float verticalProjectileAngle)
        {
            FireProjectile(projectilePrefabEnemy, original, basicForwardDirection, horizontalProjectileAngle, verticalProjectileAngle);
        }

        /// <summary>
        /// Spawns a new projectile object and sets its direction and speed
        /// </summary>
        /// <param name="original">The transform of original object used in order to sync position and orientation</param>
        void FireProjectile(GameObject prefab, Transform original, Vector3 basicForwardDirection, float horizontalProjectileAngle, float verticalProjectileAngle)
        {
            GameObject projectile = projectileFactory.Create(prefab).gameObject;
            projectile.transform.SetParent(null);
            projectile.transform.SetPositionAndRotation(original.position, original.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            Assert.IsNotNull(rb, "Rigidbody component is missing on the projectile prefab");



            var horizontalRotation = Quaternion.Euler(0, horizontalProjectileAngle, 0);
            basicForwardDirection = horizontalRotation * (basicForwardDirection.normalized);
            basicForwardDirection = Vector3.RotateTowards(basicForwardDirection, Vector3.up, verticalProjectileAngle * Mathf.Deg2Rad, 0.0f);

            rb.velocity = basicForwardDirection * projectileSettings.initialSpeed; // Initial velocity
            rb.angularVelocity = Random.insideUnitSphere * projectileSettings.angularSpeed; // Random angular velocity
        }
    }
}