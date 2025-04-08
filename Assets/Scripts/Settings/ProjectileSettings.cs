using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Settings
{
    /// <summary>
    /// Settings related to projectiles
    /// </summary>
    public static class ProjectileSettings
    {
        public static float initialSpeed = 20f;
        public static float angularSpeed = 30f;
        public static bool forceStopAfterFirstCollision = true; // If true, the projectile will stop after the first collision with the ground or an object
    }
}