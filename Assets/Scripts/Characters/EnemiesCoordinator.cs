using System.Collections;
using System.Collections.Generic;
using Projectiles.Settings;
using UnityEngine;
using Zenject;

namespace Projectiles.Characters
{
    /// <summary>
    /// Used to coordinate enemy AI and spawn enemies
    /// </summary>
    public class EnemiesCoordinator : MonoBehaviour
    {
        PlayerController playerController;

        List<EnemyController> enemies = new List<EnemyController>();

        [Inject]
        EnemyController.Factory enemyFactory;

        [Inject]
        EnemySettings enemySettings;

        [Inject]
        GameWorldSettings gameWorldSettings;

        [SerializeField]
        GameObject enemyPrefab;

        [SerializeField]
        Quaternion enemyOrientation = Quaternion.identity;


        void Start()
        {
            StartCoroutine(EnemySpawnLoop());
        }

        /// <summary>
        /// Spawns new enemies
        /// </summary>
        IEnumerator EnemySpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(enemySettings.spawnIntervalInSeconds);

                if (enemies.Count >= enemySettings.maxEnemies)
                {
                    continue;
                }

                var enemy = enemyFactory.Create(enemyPrefab);
                var enemyPos = new Vector3(
                    Random.Range(gameWorldSettings.enemyBoundsMinX, gameWorldSettings.enemyBoundsMaxX),
                    0,
                    Random.Range(gameWorldSettings.enemyBoundsMinZ, gameWorldSettings.enemyBoundsMaxZ)
                );
                enemy.transform.SetParent(transform);
                enemy.transform.SetPositionAndRotation(enemyPos, enemyOrientation);
                enemy.InitializeOnCreation(this);
            }
        }

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