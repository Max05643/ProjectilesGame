using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles.Characters
{
    /// <summary>
    /// Controls player's camera movement 
    /// </summary>
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField]
        float cameraSpeed = 5f;

        [SerializeField]
        Transform lowPos, highPos;

        [SerializeField]
        Transform cameraTransform;

        float targetPos = 0;
        float currentPos = 0;

        /// <summary>
        /// Moves camera to a higher position
        /// </summary>
        public void MoveToHighPos()
        {
            targetPos = 1;
        }

        /// <summary>
        /// Moves camera to a lower position
        /// </summary>
        public void MoveToLowPos()
        {
            targetPos = 0;
        }

        void Update()
        {
            currentPos = Mathf.MoveTowards(currentPos, targetPos, cameraSpeed * Time.deltaTime);
            cameraTransform.localPosition = Vector3.Lerp(lowPos.localPosition, highPos.localPosition, currentPos);
            cameraTransform.localRotation = Quaternion.Slerp(lowPos.localRotation, highPos.localRotation, currentPos);
        }

    }
}