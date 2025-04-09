using System;
using System.Collections;
using System.Collections.Generic;
using Projectiles.Effects;
using Projectiles.Interfaces;
using Projectiles.Settings;
using Projectiles.Utils;
using UnityEngine;
using Zenject;


namespace Projectiles.Characters
{
    /// <summary>
    /// Transfers enemy's input to GameCharacterController and handles enemy's actions
    /// </summary>
    public class EnemyController : MonoBehaviour, IDamageAble
    {

        /// <summary>
        /// Information needed for enemy AI to make decisionss and act
        /// </summary>
        class EnemyAIContext
        {
            public GameCharacterController gameCharacterController;
            public GameObject gameObject;
            public DissolveEffect dissolveEffect;
            public EnemyAICoordinator enemyAICoordinator;
            public int health = 100;
            public bool IsDead => health <= 0;
            public EnemyController controller;
            public GameWorldSettings gameWorldSettings;
            public CharacterSettings characterSettings;
            public ProjectileSettings projectileSettings;
        }


        /// <summary>
        /// Represents a state of enemy AI
        /// </summary>
        abstract class EnemyAIState
        {
            protected EnemyAIContext context;

            public EnemyAIState(EnemyAIContext context)
            {
                this.context = context;
            }

            public abstract void Tick(float deltaTime);

            /// <summary>
            /// Whether the AI can transition to another state or not
            /// </summary>
            public abstract bool CanTransition();

            /// <summary>
            /// Returns the next state to transition to. Should be called only if CanTransition() is true
            /// </summary>
            public abstract EnemyAIState TransitionToNextState();
        }


        /// <summary>
        /// Handles the death of the enemy
        /// </summary>
        class DeathState : EnemyAIState
        {
            public DeathState(EnemyAIContext context) : base(context) { }

            bool calledDeathAnimation = false;
            public override void Tick(float deltaTime)
            {
                if (calledDeathAnimation)
                {
                    return;
                }

                calledDeathAnimation = true;

                context.gameCharacterController.ChangeDeathState(true);
                context.gameCharacterController.SetMovement(Vector2.zero);
                context.dissolveEffect.HideGradually(() =>
                {
                    context.enemyAICoordinator.UnregisterEnemy(context.controller);
                    Destroy(context.gameObject);
                });
            }

            public override bool CanTransition()
            {
                return false;
            }

            public override EnemyAIState TransitionToNextState()
            {
                throw new InvalidOperationException("Cannot transition from DeathState");
            }
        }


        /// <summary>
        /// Random movement of the enemy
        /// </summary>

        class WanderState : EnemyAIState
        {
            public WanderState(EnemyAIContext context) : base(context)
            {
                duration = UnityEngine.Random.Range(3f, 10f);
            }

            Vector2 nextTargetPosition = Vector2.zero;
            float timeSinceLastPositionChange = 0f;
            float timeOnThisState = 0f;
            float duration;


            public override void Tick(float deltaTime)
            {

                timeOnThisState += deltaTime;
                timeSinceLastPositionChange += deltaTime;

                Vector2 currentPosition = new Vector2(context.gameObject.transform.position.x, context.gameObject.transform.position.z);

                if (nextTargetPosition == Vector2.zero || Vector2.Distance(currentPosition, nextTargetPosition) < 0.5f || timeSinceLastPositionChange > 10f)
                {
                    timeSinceLastPositionChange = 0f;
                    nextTargetPosition = new Vector2(
                        UnityEngine.Random.Range(context.gameWorldSettings.enemyBoundsMinX, context.gameWorldSettings.enemyBoundsMaxX),
                        UnityEngine.Random.Range(context.gameWorldSettings.enemyBoundsMinZ, context.gameWorldSettings.enemyBoundsMaxZ));
                }

                Vector2 dir = (nextTargetPosition - currentPosition).normalized;

                foreach (var otherAI in context.enemyAICoordinator.GetEnemiesNearMe(context.gameObject.transform.position))
                {
                    var otherAIPos = new Vector2(otherAI.x, otherAI.z);
                    var dist = Vector2.Distance(currentPosition, otherAIPos);
                    dir += (currentPosition - otherAIPos).normalized * (1f / (dist + 0.05f));
                }

                context.gameCharacterController.SetMovementWorldSpace(dir);
            }

            public override bool CanTransition()
            {
                var playerPos = context.enemyAICoordinator.GetPlayersWorldPosition();
                return context.IsDead || (timeOnThisState > duration && playerPos.HasValue && Vector3.Distance(context.gameObject.transform.position, playerPos.Value) <= context.characterSettings.AIAttackRange);
            }

            public override EnemyAIState TransitionToNextState()
            {
                var playerPos = context.enemyAICoordinator.GetPlayersWorldPosition();

                if (context.IsDead)
                {
                    return new DeathState(context);
                }
                else if ((timeOnThisState > duration && playerPos.HasValue && Vector3.Distance(context.gameObject.transform.position, playerPos.Value) <= context.characterSettings.AIAttackRange))
                {
                    return new AttackState(context);
                }
                else
                {
                    throw new InvalidOperationException("Cannot transition from WanderState");
                }
            }
        }


        class AttackState : EnemyAIState
        {
            public AttackState(EnemyAIContext context) : base(context) { }

            bool startedAttack = false;
            bool thrownProjectile = false;
            bool endedAttack = false;

            public override void Tick(float deltaTime)
            {
                if (!startedAttack)
                {
                    startedAttack = true;
                    context.gameCharacterController.ChangeAttackPrepareState(true);
                    context.gameCharacterController.SetMovement(Vector2.zero);
                }
                else if (!thrownProjectile)
                {
                    const float playerPreemptingTime = 1.5f;

                    var playerPos = context.enemyAICoordinator.GetPlayersWorldPosition();
                    var playerVelocity = context.enemyAICoordinator.GetPlayersWorldVelocity();

                    if (!playerPos.HasValue || !playerVelocity.HasValue)
                    {
                        Debug.LogError("Player position or velocity is not available");
                        endedAttack = true;
                        return;
                    }

                    thrownProjectile = true;

                    var basicForwardDirection = context.gameObject.transform.forward;
                    var firePosition = context.gameCharacterController.ProjectileSpawnPoint.position;

                    var targetPosition = playerPos.Value + playerVelocity.Value * playerPreemptingTime;
                    var vectorToPlayer = targetPosition - firePosition;

                    var distance = vectorToPlayer.magnitude;

                    var horizontalProjectileAngle = Vector3.SignedAngle(basicForwardDirection, vectorToPlayer, Vector3.up);

                    var verticalProjectileAngle = BallisticCalculator.CalculateProjectileTargetVerticalAngle(
                        firePosition,
                        basicForwardDirection,
                        distance,
                        context.projectileSettings.initialSpeed
                    );


                    if (verticalProjectileAngle.HasValue)
                    {
                        context.gameCharacterController.ThrowProjectile(
                            horizontalProjectileAngle,
                            verticalProjectileAngle.Value
                        );
                    }
                    else
                    {
                        endedAttack = true;
                    }

                    context.gameCharacterController.ChangeAttackPrepareState(false);

                    context.gameCharacterController.onProjectileThrown.AddListener(OnAnimationEnd);
                }
            }

            void OnAnimationEnd()
            {
                endedAttack = true;
            }

            public override bool CanTransition()
            {
                return context.IsDead || endedAttack;
            }

            public override EnemyAIState TransitionToNextState()
            {
                context.gameCharacterController.onProjectileThrown.RemoveListener(OnAnimationEnd);

                if (context.IsDead)
                {
                    return new DeathState(context);
                }
                else if (endedAttack)
                {
                    return new WanderState(context);
                }
                else
                {
                    throw new InvalidOperationException("Cannot transition from AttackState");
                }
            }
        }


        EnemyAIContext enemyAIContext;
        EnemyAIState currentState;

        [Inject]
        void Inject(EnemyAICoordinator enemyAICoordinator, GameWorldSettings gameWorldSettings, CharacterSettings characterSettings, ProjectileSettings projectileSettings)
        {
            enemyAIContext = new EnemyAIContext()
            {
                gameCharacterController = GetComponent<GameCharacterController>(),
                dissolveEffect = GetComponent<DissolveEffect>(),
                enemyAICoordinator = enemyAICoordinator,
                gameObject = gameObject,
                gameWorldSettings = gameWorldSettings,
                controller = this,
                characterSettings = characterSettings,
                projectileSettings = projectileSettings
            };

            enemyAICoordinator.RegisterEnemy(this);

            currentState = new WanderState(enemyAIContext);
        }

        void IDamageAble.ApplyDamage(int damage)
        {
            enemyAIContext.health -= damage;
            enemyAIContext.health = Mathf.Clamp(enemyAIContext.health, 0, 100);
        }


        void Update()
        {
            if (currentState.CanTransition())
            {
                currentState = currentState.TransitionToNextState();
            }

            currentState.Tick(Time.deltaTime);
        }
    }
}