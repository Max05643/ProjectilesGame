using System;
using System.Collections;
using System.Collections.Generic;
using Projectiles.Interfaces;
using Projectiles.Settings;
using Projectiles.UI;
using Projectiles.Utils;
using UnityEngine;
using Zenject;


namespace Projectiles.Characters
{
    /// <summary>
    /// Transfers player's input to GameCharacterController and handles player's actions
    /// </summary>
    public class PlayerController : MonoBehaviour, IInitializable
    {
        [SerializeField]
        GameCharacterController gameCharacterController;

        [Inject]
        TrajectoryDisplayer trajectoryDisplayer;

        [SerializeField]
        float minHorizontalAngle = -30f, maxHorizontalAngle = 30f;

        [SerializeField]
        float minVerticalAngle = 0f, maxVerticalAngle = 30f;

        [Inject]
        ProjectileSettings projectileSettings;

        [Inject]
        CharacterSettings characterSettings;

        [Inject]
        EnemiesCoordinator enemyAICoordinator;

        [Inject]
        GameWorldSettings gameWorldSettings;

        float xAxis = 0f;
        float yAxis = 0f;
        float horizontalProjectileAngle = 0;
        float verticalProjectileAngle = 0;

        public bool CanPrepareToAttackAgain => gameCharacterController.CurrentAnimationState != GameCharacterController.AnimationState.Hit && gameCharacterController.CurrentAnimationState != GameCharacterController.AnimationState.Throwing && !gameCharacterController.WasFireRequested;
        public bool IsAttackPrepared => gameCharacterController.CurrentAnimationState == GameCharacterController.AnimationState.Preparing;
        public bool CanMove => gameCharacterController.CurrentAnimationState == GameCharacterController.AnimationState.Movement;

        public void StartPreparingAttack()
        {
            gameCharacterController.ChangeAttackPrepareState(true);
        }

        public void StopPreparingAttack()
        {
            gameCharacterController.ChangeAttackPrepareState(false);
        }

        public void SetXAxisNormalized(float value)
        {
            xAxis = Mathf.Lerp(-1f, 1f, value);
        }

        public void SetYAxisNormalized(float value)
        {
            yAxis = Mathf.Lerp(-1f, 1f, value);
        }

        public void SetNormalizedHorizontalProjectileAngle(float value)
        {
            horizontalProjectileAngle = Mathf.Lerp(minHorizontalAngle, maxHorizontalAngle, value);
        }

        public void SetNormalizedVerticalProjectileAngle(float value)
        {
            verticalProjectileAngle = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, value);
        }

        void IInitializable.Initialize()
        {
            enemyAICoordinator.RegisterPlayer(this);
            gameCharacterController.SetHealthAndMaxHealth(characterSettings.maxPlayerHealth, characterSettings.maxPlayerHealth);
        }

        public void Fire()
        {
            if (IsAttackPrepared)
            {
                gameCharacterController.ThrowProjectile(horizontalProjectileAngle, verticalProjectileAngle);
            }
        }

        /// <summary>
        /// Restricts player from going out of the world bounds
        /// </summary>
        Vector2 RestrictMovementsInWorldBounds(Vector2 movementVectorObjectSpace)
        {
            var playerPosWorldSpace = gameCharacterController.transform.position;
            var vectorWorldSpace = gameCharacterController.transform.TransformDirection(new Vector3(movementVectorObjectSpace.x, 0, movementVectorObjectSpace.y));

            if (playerPosWorldSpace.x <= gameWorldSettings.playerBoundsMinX && vectorWorldSpace.x < 0f)
            {
                vectorWorldSpace.x = 0f;
            }
            else if (playerPosWorldSpace.x >= gameWorldSettings.playerBoundsMaxX && vectorWorldSpace.x > 0f)
            {
                vectorWorldSpace.x = 0f;
            }

            if (playerPosWorldSpace.z <= gameWorldSettings.playerBoundsMinZ && vectorWorldSpace.z < 0f)
            {
                vectorWorldSpace.z = 0f;
            }
            else if (playerPosWorldSpace.z >= gameWorldSettings.playerBoundsMaxZ && vectorWorldSpace.z > 0f)
            {
                vectorWorldSpace.z = 0f;
            }



            var restrictedVectorObjectSpace = gameCharacterController.transform.InverseTransformDirection(vectorWorldSpace);
            return new Vector2(restrictedVectorObjectSpace.x, restrictedVectorObjectSpace.z);
        }

        void Update()
        {
            if (!CanMove)
            {
                gameCharacterController.SetMovement(Vector2.zero);
            }
            else
            {
                var movementVector = new Vector2(xAxis, yAxis);

                if (movementVector.magnitude > 1f)
                {
                    movementVector.Normalize();
                }

                gameCharacterController.SetMovement(RestrictMovementsInWorldBounds(movementVector));
            }

            // If we are in an attack state, we show the projectile trajectory
            if (IsAttackPrepared)
            {
                trajectoryDisplayer.Show(
                    BallisticCalculator.CalculateProjectileTrajectory(
                        gameCharacterController.ProjectileSpawnPoint.position,
                        gameCharacterController.transform.forward,
                        horizontalProjectileAngle,
                        verticalProjectileAngle,
                        true,
                        false,
                        projectileSettings.initialSpeed
                    )
                );
            }
            else
            {
                trajectoryDisplayer.Hide();
            }
        }
    }
}