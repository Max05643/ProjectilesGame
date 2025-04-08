using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Projectiles.Utils
{
    /// <summary>
    /// Contains utility functions for manipulating GameObjects
    /// </summary>
    public static class GameObjectUtils
    {
        /// <summary>
        /// Ensures that the list has at least the specified number of elements by instantiating the prefab and adding it to the list
        /// </summary>
        public static void PopulateListWithObjects(this List<GameObject> list, GameObject prefab, int count, Transform parent)
        {
            Assert.IsNotNull(prefab, $"Prefab cannot be null in {nameof(PopulateListWithObjects)}");

            while (list.Count < count)
            {
                GameObject obj = Object.Instantiate(prefab, parent);
                list.Add(obj);
            }
        }
    }
}