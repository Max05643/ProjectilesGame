using System.Collections;
using System.Collections.Generic;
using Projectiles.Interfaces;
using Projectiles.Settings;
using UnityEngine;
using Zenject;

namespace Projectiles.Projectiles
{
    /// <summary>
    /// Controles projectile's behaviour (e.g. collision, damage, etc.) 
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<UnityEngine.Object, ProjectileController>
        {
        }

        [Inject]
        ProjectileSettings projectileSettings;

        /// <summary>
        /// The time in seconds after which the projectile will be destroyed if it doesn't hit anything
        /// </summary>
        [SerializeField]
        float lifeTimeAfterGroundCollision = 5f;

        [SerializeField]
        Rigidbody rg;

        float? timeOfCollision = null; // The time of collision with the ground


        void Update()
        {
            if (timeOfCollision != null && Time.timeSinceLevelLoad - timeOfCollision > lifeTimeAfterGroundCollision)
            {
                Destroy(gameObject);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            gameObject.layer = LayerMask.NameToLayer("CollidedProjectile");

            if (projectileSettings.forceStopAfterFirstCollision)
            {
                rg.velocity = Vector3.zero; // Stop the projectile on collision
            }

            if (timeOfCollision.HasValue)
            {
                return; // Multiple collisions are not allowed
            }
            else
            {
                timeOfCollision = Time.timeSinceLevelLoad;
            }

            var damageable = collision.gameObject.GetComponent<IDamageAble>();

            if (damageable != null)
            {
                damageable.ApplyDamage(projectileSettings.damage);
                Destroy(gameObject);
            }
        }
    }
}