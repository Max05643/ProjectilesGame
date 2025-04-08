using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Characters
{
    /// <summary>
    /// Transfers player's input to GameCharacterController and handles player's actions
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        GameCharacterController gameCharacterController;

        [SerializeField]
        [Range(-30, 30)]
        float horizontalProjectileAngle = 0;

        [SerializeField]
        [Range(0, 30)]
        float verticalProjectileAngle = 0;

        [SerializeField]
        [Range(-1, 1)]
        float xAxis, yAxis;

        [SerializeField]
        bool isDead;

        [SerializeField]
        bool attackPrepare;

        [ContextMenu("Test Attack")]
        void TestAttack()
        {
            gameCharacterController.ThrowProjectile(horizontalProjectileAngle, verticalProjectileAngle);
        }


        void Update()
        {
            var movementVector = new Vector2(xAxis, yAxis);

            if (movementVector.magnitude > 1f)
            {
                movementVector.Normalize();
            }

            gameCharacterController.SetMovement(movementVector);
            gameCharacterController.ChangeAttackPrepareState(attackPrepare);
            gameCharacterController.ChangeDeathState(isDead);
        }
    }
}