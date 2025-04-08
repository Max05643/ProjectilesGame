using System;
using System.Collections;
using System.Collections.Generic;
using Projectiles.Settings;
using Projectiles.UI;
using Projectiles.Utils;
using UnityEngine;


namespace Projectiles.Characters
{
    /// <summary>
    /// Transfers player's input to GameCharacterController and handles player's actions
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        GameCharacterController gameCharacterController;

        [SerializeField]
        TrajectoryDisplayer trajectoryDisplayer;

        [SerializeField]
        float minHorizontalAngle = -30f, maxHorizontalAngle = 30f;

        [SerializeField]
        float minVerticalAngle = 0f, maxVerticalAngle = 30f;

        bool attackPrepared = false;
        float xAxis = 0f;
        float yAxis = 0f;
        float horizontalProjectileAngle = 0;
        float verticalProjectileAngle = 0;


        public bool IsAttackPrepared => attackPrepared;

        public void StartPreparingAttack()
        {
            attackPrepared = true;
            gameCharacterController.ChangeAttackPrepareState(attackPrepared);
        }

        public void StopPreparingAttack()
        {
            attackPrepared = false;
            gameCharacterController.ChangeAttackPrepareState(attackPrepared);
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

        public void Fire()
        {
            if (attackPrepared)
            {
                gameCharacterController.ThrowProjectile(horizontalProjectileAngle, verticalProjectileAngle);
            }
        }

        void Update()
        {
            var movementVector = new Vector2(xAxis, yAxis);

            if (movementVector.magnitude > 1f)
            {
                movementVector.Normalize();
            }

            gameCharacterController.SetMovement(movementVector);

            // If we are in an attack state, we show the projectile trajectory
            if (attackPrepared)
            {
                trajectoryDisplayer.Show(
                    BallisticCalculator.CalculateProjectileTrajectory(
                        gameCharacterController.ProjectileSpawnPoint.position,
                        gameCharacterController.transform.forward,
                        horizontalProjectileAngle,
                        verticalProjectileAngle,
                        true,
                        false,
                        ProjectileSettings.initialSpeed
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