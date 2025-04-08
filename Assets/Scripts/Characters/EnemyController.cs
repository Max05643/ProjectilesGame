using System.Collections;
using System.Collections.Generic;
using Projectiles.Effects;
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
        DissolveEffect dissolveEffect;

        [SerializeField]
        float timeToDisappear = 5f;

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
            dissolveEffect.ChangeEffectGradually(1f, timeToDisappear, () =>
            {
                Destroy(gameObject);
            });
        }

    }
}