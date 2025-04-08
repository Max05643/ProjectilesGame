using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Interfaces
{
    /// <summary>
    /// Interface for objects that can be damaged
    /// </summary>
    public interface IDamageAble
    {
        /// <summary>
        /// Applies damage to the object
        /// </summary>
        void ApplyDamage(int damage);
    }
}