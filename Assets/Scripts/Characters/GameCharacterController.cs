using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Projectiles.Interfaces;
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
    public class GameCharacterController : MonoBehaviour, IDamageAble
    {
        public enum AnimationState
        {
            Throwing,
            Preparing,
            Dead,
            Movement,
            Hit
        }

        [SerializeField]
        Animator animator;

        [SerializeField]
        CharacterSoundController soundController;

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

        public UnityEvent onHealthChanged = new UnityEvent();
        public UnityEvent onDeath = new UnityEvent();
        /// <summary>
        /// Event that is called when the projectile is thrown
        /// </summary>
        public UnityEvent onProjectileThrown = new UnityEvent();
        /// <summary>
        /// Event that is called when the character is hit by projectile
        /// </summary>
        public UnityEvent onGotHit = new UnityEvent();
        [SerializeField]
        int health = 100;
        [SerializeField]
        int maxHealth = 100;
        public bool IsDead => health <= 0;
        public float HealthNormalized => (float)health / maxHealth;


        /// <summary>
        /// Character's animation state
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
        /// Sets character's health and max health
        /// </summary>
        public void SetHealthAndMaxHealth(int health, int maxHealth)
        {
            this.health = health;
            this.maxHealth = maxHealth;

            ChangeDeathState(health <= 0);
            animator.ResetTrigger("Hit");
            onHealthChanged.Invoke();
        }

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
        void ChangeDeathState(bool isDead)
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
            if (CurrentAnimationState == AnimationState.Movement)
            {
                currentMovementDirection = Vector2.MoveTowards(currentMovementDirection, targetMovementDirection, characterSettings.accelerationNormalized * Time.deltaTime);
            }
            else
            {
                currentMovementDirection = Vector2.MoveTowards(currentMovementDirection, Vector2.zero, characterSettings.accelerationNormalized * Time.deltaTime);
            }

            var velocityObjectSpace = new Vector3(currentMovementDirection.x, 0, currentMovementDirection.y) * characterSettings.maxMovementSpeed;
            rb.velocity = transform.TransformDirection(velocityObjectSpace);

            animator.SetFloat("DirectionXAxis", currentMovementDirection.x);
            animator.SetFloat("DirectionYAxis", currentMovementDirection.y);
        }


        /// <summary>
        /// Called from the animation on the frame where projectile is almost thrown
        /// </summary>
        public void ProjectileWillBeThrown()
        {
            soundController.PlayThrowSound();
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

            requestedFire = false;
            projectileAnimationObject.SetActive(false);
            onProjectileThrown.Invoke();
        }

        /// <summary>
        /// Called from the animation on the frame where projectile should shown again
        /// </summary>
        public void ProjectileActivated()
        {
            projectileAnimationObject.SetActive(true);
        }


        /// <summary>
        /// Plays the hit animation. Should be called when character is hit by a projectile
        /// </summary>
        void AddHitAnimation()
        {
            animator.SetTrigger("Hit");
        }

        /// <summary>
        /// Called when projectile hits character
        /// </summary>
        void IDamageAble.ApplyDamage(int damage)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, maxHealth);
            onHealthChanged.Invoke();

            if (IsDead)
            {
                ChangeDeathState(true);
                onDeath.Invoke();
                soundController.PlayDeadSound();
            }
            else
            {
                animator.ResetTrigger("AttackDo");
                requestedFire = false;
                AddHitAnimation();
                soundController.PlayHitSound();
                onGotHit.Invoke();
            }
        }
    }
}