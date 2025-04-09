using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Settings
{

    /// <summary>
    /// Settings related to the game world
    /// </summary>
    [Serializable]
    public class GameWorldSettings
    {
        public float enemyBoundsMinX, enemyBoundsMaxX, enemyBoundsMinZ, enemyBoundsMaxZ;
        public float playerBoundsMinX, playerBoundsMaxX, playerBoundsMinZ, playerBoundsMaxZ;
    }
}