using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Projectiles.Physics;
using Projectiles.Settings;
using UnityEngine;
using Zenject;

namespace Projectiles.Characters
{
    /// <summary>
    /// Used to control game characters (player and enemies)
    /// </summary>
    public class GameCharacterController : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        Rigidbody rb;

        [SerializeField]
        GameObject projectileAnimationObject;

        [Inject]
        ProjectilesCreator projectilesCreator;

        [Inject]
        CharacterSettings characterSettings;

        [SerializeField]
        Transform projectileSpawnPoint; // The point where the projectile is spawned

        Vector2 targetMovementDirection = Vector2.zero;
        Vector2 currentMovementDirection = Vector2.zero;

        float lastHorizontalProjectileAngle = 0f;
        float lastVerticalProjectileAngle = 0f;


        public Transform ProjectileSpawnPoint => projectileSpawnPoint;

        /// <summary>
        /// Plays the projectile throwing animation. Should be called only if attackPrepare is set
        /// </summary>
        public void ThrowProjectile(float horizontalProjectileAngle, float verticalProjectileAngle)
        {
            if (animator.GetBool("IsDead"))
            {
                Debug.LogError("Character is dead!");
            }
            else if (!animator.GetBool("AttackPrepare"))
            {
                Debug.LogError("Attack not prepared!");
            }
            else
            {
                animator.SetTrigger("AttackDo");
                lastHorizontalProjectileAngle = horizontalProjectileAngle;
                lastVerticalProjectileAngle = verticalProjectileAngle;
            }
        }

        /// <summary>
        /// Changes character's death status
        /// </summary>
        public void ChangeDeathState(bool isDead)
        {
            animator.SetBool("IsDead", isDead);
        }

        /// <summary>
        /// Changes character's attack preparation status
        /// </summary>
        public void ChangeAttackPrepareState(bool attackPrepare)
        {
            animator.SetBool("AttackPrepare", attackPrepare);
        }

        /// <summary>
        /// Sets the character's movement direction
        /// </summary>
        public void SetMovement(Vector2 direction)
        {
            targetMovementDirection = direction;
        }

        void Update()
        {
            currentMovementDirection = Vector2.MoveTowards(currentMovementDirection, targetMovementDirection, characterSettings.accelerationNormalized * Time.deltaTime);


            var velocityObjectSpace = new Vector3(currentMovementDirection.x, 0, currentMovementDirection.y) * characterSettings.maxMovementSpeed;
            rb.velocity = transform.TransformDirection(velocityObjectSpace);
            animator.SetFloat("DirectionXAxis", currentMovementDirection.x);
            animator.SetFloat("DirectionYAxis", currentMovementDirection.y);
        }

        /// <summary>
        /// Called from the animation on the frame where projectile should be converted to a physical object
        /// </summary>
        public void ProjectileThrown()
        {
            projectilesCreator.FirePlayersProjectile(projectileAnimationObject.transform, transform.forward, lastHorizontalProjectileAngle, lastVerticalProjectileAngle);
            projectileAnimationObject.SetActive(false);
        }

        /// <summary>
        /// Called from the animation on the frame where projectile should shown again
        /// </summary>
        public void ProjectileActivated()
        {
            projectileAnimationObject.SetActive(true);
        }
    }
}