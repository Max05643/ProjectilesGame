using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles.Utils
{
    /// <summary>
    /// Used to calculate the trajectory of a projectile
    /// </summary>
    public static class BallisticCalculator
    {
        static LayerMask CalculateLayerMask(bool shouldHitEnemy, bool shouldHitPlayer)
        {
            LayerMask layerMask = 0;
            if (shouldHitEnemy)
            {
                layerMask |= LayerMask.GetMask("Enemy");
            }
            if (shouldHitPlayer)
            {
                layerMask |= LayerMask.GetMask("Player");
            }

            return layerMask | LayerMask.GetMask("Ground");
        }


        /// <summary>
        /// Calculates whether the ray from rayStart to rayEnd hits something in the specified layerMask
        /// </summary>
        static bool HitSomething(Vector3 rayStart, Vector3 rayEnd, LayerMask layerMask, out Vector3 hitPosition)
        {
            hitPosition = Vector3.zero;
            Ray ray = new Ray(rayStart, rayEnd - rayStart);
            RaycastHit hitInfo;
            if (UnityEngine.Physics.Raycast(ray, out hitInfo, Vector3.Distance(rayStart, rayEnd), layerMask))
            {
                hitPosition = hitInfo.point;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the approximate trajectory of a projectile in 3D space and returns the points of the trajectory.
        /// Stops calculating when it hits something in the specified layerMask or when Y is less than or equal to 0.
        /// </summary>
        public static List<Vector3> CalculateProjectileTrajectory(Vector3 startPosition, Vector3 basicForwardDirection, float horizontalProjectileAngle, float verticalProjectileAngle, bool shouldHitEnemy, bool shouldHitPlayer, float initialSpeed)
        {
            Vector3 gravity = UnityEngine.Physics.gravity;
            var layerMask = CalculateLayerMask(shouldHitEnemy, shouldHitPlayer);
            const float timeStep = 0.1f;
            const float maxSimulationTime = 10f;

            var rotation = Quaternion.Euler(-verticalProjectileAngle, horizontalProjectileAngle, 0);
            basicForwardDirection = rotation * (basicForwardDirection.normalized);

            var initialVelocity = basicForwardDirection * initialSpeed;

            List<Vector3> trajectoryPoints = new List<Vector3>();
            Vector3 position = startPosition;
            Vector3 velocity = initialVelocity;

            trajectoryPoints.Add(position);

            for (float time = 0f; time < maxSimulationTime; time += timeStep)
            {
                Vector3 nextPosition = position + velocity * timeStep + 0.5f * gravity * timeStep * timeStep;
                Vector3 hitPos;

                // Check for collision between current and next position
                if (HitSomething(position, nextPosition, layerMask, out hitPos))
                {
                    trajectoryPoints.Add(Vector3.Lerp(position, hitPos, 0.5f));
                    break;
                }

                position = nextPosition;
                velocity += gravity * timeStep;
                trajectoryPoints.Add(position);

                if (position.y <= 0f)
                {
                    break;
                }
            }

            return trajectoryPoints;
        }
    }
}
