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

        [SerializeField]
        GameCharacterController gameCharacterController;

        [SerializeField]
        float timeToDisappear = 5f;

        float? timeOfDeath = null;

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
            gameCharacterController.ChangeDeathState(true);
            gameCharacterController.SetMovement(Vector2.zero);
            timeOfDeath = Time.timeSinceLevelLoad;
        }

        void Update()
        {
            if (timeOfDeath != null && Time.timeSinceLevelLoad - timeOfDeath > timeToDisappear)
            {
                Destroy(gameObject);
            }
        }
    }
}