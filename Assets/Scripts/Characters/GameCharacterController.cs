using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Projectiles.Physics;
using Projectiles.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Projectiles.Characters
{
    /// <summary>
    /// Used to control game characters (player and enemies)
    /// </summary>
    public class GameCharacterController : MonoBehaviour
    {
        public enum AnimationState
        {
            Throwing,
            Preparing,
            Dead,
            Movement
        }

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

        [SerializeField]
        bool isPlayer = true;

        Vector2 targetMovementDirection = Vector2.zero;
        Vector2 currentMovementDirection = Vector2.zero;

        float lastHorizontalProjectileAngle = 0f;
        float lastVerticalProjectileAngle = 0f;

        /// <summary>
        /// Character's animation state. Can be used to sync actions to animations 
        /// </summary>
        public AnimationState CurrentAnimationState
        {
            get
            {
                foreach (var state in (AnimationState[])Enum.GetValues(typeof(AnimationState)))
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsTag(state.ToString()))
                    {
                        return state;
                    }
                }

                throw new NotImplementedException($"Unknown animation state in {nameof(GameCharacterController)} in {gameObject.name}");
            }
        }

        bool requestedFire = false;
        public bool WasFireRequested => requestedFire;

        public Transform ProjectileSpawnPoint => projectileSpawnPoint;

        /// <summary>
        /// Event that is called when the projectile is thrown. Can be used to sync actions to animations
        /// </summary>
        public UnityEvent onProjectileThrown = new UnityEvent();

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
                requestedFire = true;
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
        /// Sets the character's movement direction in object space (x, z). Direction is not normalized
        /// </summary>
        public void SetMovement(Vector2 direction)
        {
            targetMovementDirection = direction;
        }

        /// <summary>
        /// Sets the character's movement direction in world space (x, z). Direction is normalized
        /// </summary>
        public void SetMovementWorldSpace(Vector2 direction)
        {
            var transformed = transform.InverseTransformDirection(new Vector3(direction.x, 0, direction.y)).normalized;
            targetMovementDirection = new Vector2(transformed.x, transformed.z);
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
            if (isPlayer)
            {
                projectilesCreator.FirePlayersProjectile(projectileAnimationObject.transform, transform.forward, lastHorizontalProjectileAngle, lastVerticalProjectileAngle);
            }
            else
            {
                projectilesCreator.FireEnemiesProjectile(projectileAnimationObject.transform, transform.forward, lastHorizontalProjectileAngle, lastVerticalProjectileAngle);
            }


            projectileAnimationObject.SetActive(false);

            onProjectileThrown.Invoke();
        }

        /// <summary>
        /// Called from the animation on the frame where projectile should shown again
        /// </summary>
        public void ProjectileActivated()
        {
            projectileAnimationObject.SetActive(true);
            requestedFire = false;
        }
    }
}