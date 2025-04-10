using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Settings
{
    /// <summary>
    /// Settings related to enemies
    /// </summary>
    [Serializable]
    public class EnemySettings
    {
        public int maxEnemies = 5;
        public float spawnIntervalInSeconds = 5f;
    }
}