using System.Collections;
using System.Collections.Generic;
using Projectiles.Utils;
using UnityEngine;

namespace Projectiles.UI
{
    /// <summary>
    /// Used to display the trajectory of a projectile
    /// </summary>
    public class TrajectoryDisplayer : MonoBehaviour
    {

        [SerializeField]
        GameObject pointPrefab;

        List<GameObject> points = new List<GameObject>();


        /// <summary>
        /// Hides the trajectory
        /// </summary>
        public void Hide()
        {
            foreach (var point in points)
            {
                point.SetActive(false);
            }
        }


        public void Show(List<Vector3> trajectory)
        {
            GameObjectUtils.PopulateListWithObjects(points, pointPrefab, trajectory.Count, transform);
            Hide();
            for (int i = 0; i < trajectory.Count; i++)
            {
                points[i].SetActive(true);
                points[i].transform.position = trajectory[i];
            }
        }


    }
}