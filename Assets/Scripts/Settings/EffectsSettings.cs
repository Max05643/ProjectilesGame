using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Settings
{
    /// <summary>
    /// Settings related to visual effects
    /// </summary>
    [Serializable]
    public class EffectsSettings
    {
        /// <summary>
        /// Time in seconds for the destroyed object to disappear
        /// </summary>
        public float timeForItemToDisappear = 2f;

        /// <summary>
        /// Time in seconds for the new object to appear
        /// </summary>
        public float timeForItemToAppear = 2f;
    }
}