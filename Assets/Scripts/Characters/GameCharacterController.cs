using System.Collections;
using System.Collections.Generic;
using Projectiles.Physics;
using UnityEngine;

namespace Projectiles.Characters
{
    /// <summary>
    /// Used to control game characters (player and enemies)
    /// </summary>
    public class GameCharacterController : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        [Range(-1, 1)]
        float xAxis, yAxis;

        [SerializeField]
        bool isDead;

        [SerializeField]
        bool attackPrepare;

        [SerializeField]
        GameObject projectileAnimationObject;

        [SerializeField]
        ProjectilesCreator projectilesCreator;


        [ContextMenu("Throw projectile")]
        public void ThrowProjectile()
        {
            if (!attackPrepare)
            {
                Debug.LogError("Attack not prepared!");
            }
            else
            {
                animator.SetTrigger("AttackDo");
            }
        }

        void Update()
        {
            animator.SetFloat("DirectionXAxis", xAxis);
            animator.SetFloat("DirectionYAxis", yAxis);
            animator.SetBool("IsDead", isDead);
            animator.SetBool("AttackPrepare", attackPrepare);
        }

        /// <summary>
        /// Called from the animation on the frame where projectile should be converted to a physical object
        /// </summary>
        public void ProjectileThrown()
        {
            projectilesCreator.FirePlayersProjectile(projectileAnimationObject.transform, transform.forward);
            projectileAnimationObject.SetActive(false);
        }

        /// <summary>
        /// Called from the animation on the frame where projectile should shown again
        /// </summary>
        public void ProjectileActivated()
        {
            projectileAnimationObject.SetActive(true);
        }
    }
}