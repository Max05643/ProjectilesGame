using System.Collections;
using System.Collections.Generic;
using Projectiles.Interfaces;
using UnityEngine;


namespace Projectiles.Characters
{
    /// <summary>
    /// Transfers enemy's input to GameCharacterController and handles enemy's actions
    /// </summary>
    public class EnemyController : MonoBehaviour, IDamageAble
    {
        [SerializeField]
        int health = 100;


        void IDamageAble.ApplyDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Enemy died!");
            Destroy(gameObject);
        }
    }
}