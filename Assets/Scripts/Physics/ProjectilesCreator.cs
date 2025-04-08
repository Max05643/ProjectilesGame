using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace Projectiles.Physics
{
    public class ProjectilesCreator : MonoBehaviour
    {
        [SerializeField]
        GameObject projectilePrefabPlayer, projectilePrefabEnemy;

        [SerializeField]
        float initialSpeed = 50f, rotationSpeed = 100f;

        /// <summary>
        /// Creates a projectile object which belongs to player and sets its direction and speed
        /// </summary>
        public void FirePlayersProjectile(Transform original, Vector3 direction)
        {
            FireProjectile(projectilePrefabPlayer, original, direction);
        }

        /// <summary>
        /// Creates a projectile object which belongs to enemy and sets its direction and speed
        /// </summary>
        public void FireEnemiesProjectile(Transform original, Vector3 direction)
        {
            FireProjectile(projectilePrefabEnemy, original, direction);
        }

        /// <summary>
        /// Spawns a new projectile object and sets its direction and speed
        /// </summary>
        /// <param name="original">The transform of original object used in order to sync position and orientation</param>
        void FireProjectile(GameObject prefab, Transform original, Vector3 direction)
        {
            GameObject projectile = Instantiate(prefab, original.position, original.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            Assert.IsNotNull(rb, "Rigidbody component is missing on the projectile prefab");

            rb.velocity = direction.normalized * initialSpeed; // Initial velocity
            rb.angularVelocity = Random.insideUnitSphere * rotationSpeed; // Random rotation
        }
    }
}