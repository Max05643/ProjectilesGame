using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles.Characters
{
    /// <summary>
    /// Used to coordinate enemy AI
    /// </summary>
    public class EnemyAICoordinator : MonoBehaviour
    {
        PlayerController playerController;

        List<EnemyController> enemies = new List<EnemyController>();


        /// <summary>
        /// Returna a collection of enemies positions near the given position in world space
        /// </summary>
        public IEnumerable<Vector3> GetEnemiesNearMe(Vector3 pos)
        {
            const float threshold = 2f;

            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(enemy.transform.position, pos);
                if (distance < threshold)
                {
                    yield return enemy.transform.position;
                }
            }
        }

        /// <summary>
        /// Returns the velocity of the player in world space or null if player is not registered
        /// </summary>
        public Vector3? GetPlayersWorldVelocity()
        {
            if (playerController == null)
            {
                return null;
            }
            return playerController.GetComponent<Rigidbody>().velocity;
        }

        /// <summary>
        /// Returns the position of the player in world space or null if player is not registered
        /// </summary>
        public Vector3? GetPlayersWorldPosition()
        {
            if (playerController == null)
            {
                return null;
            }
            return playerController.transform.position;
        }

        /// <summary>
        /// Registers player
        /// </summary>
        public void RegisterPlayer(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        /// <summary>
        /// Adds enemy to the list of enemies
        /// </summary>
        public void RegisterEnemy(EnemyController enemyController)
        {
            enemies.Add(enemyController);
        }


        /// <summary>
        /// Removes enemy from the list of enemies
        /// </summary>
        public void UnregisterEnemy(EnemyController enemyController)
        {
            enemies.Remove(enemyController);
        }

    }
}