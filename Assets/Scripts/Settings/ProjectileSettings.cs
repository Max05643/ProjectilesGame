using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Settings
{
    /// <summary>
    /// Settings related to projectiles
    /// </summary>
    [Serializable]
    public class ProjectileSettings
    {
        [Range(1, 100)]
        public int damage = 10; // Damage dealt by the projectile
        [Min(0)]
        public float initialSpeed = 20f;
        [Min(0)]
        public float angularSpeed = 30f;
        public bool forceStopAfterFirstCollision = true; // If true, the projectile will stop after the first collision with the ground or an object
    }
}