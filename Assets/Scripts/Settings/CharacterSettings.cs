using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles.Settings
{
    [Serializable]
    public class CharacterSettings
    {
        public float maxMovementSpeed = 5f;
        public float accelerationNormalized = 0.5f;
        public float AIAttackRange = 10f;

        public int maxPlayerHealth = 1000;
        public int maxEnemyHealth = 100;

    }
}