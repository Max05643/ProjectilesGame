using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Projectiles.Effects
{
    /// <summary>
    /// Uses disovle shader to create a dissolve effect on the object
    /// </summary>
    public class DissolveEffect : MonoBehaviour
    {
        [SerializeField]
        Renderer targetRenderer;

        Tween currentAnimation;


        void OnDestroy()
        {
            if (currentAnimation != null)
            {
                currentAnimation.Kill();
            }
        }

        /// <summary>
        /// Shows the object immediately
        /// </summary>
        public void ShowImmediate()
        {
            targetRenderer.material.SetFloat("_Progress", 0f);
        }

        /// <summary>
        /// Hides the object immediately
        /// </summary>
        public void HideImmediate()
        {
            targetRenderer.material.SetFloat("_Progress", 1f);
        }

        /// <summary>
        /// Shows/Hides the object gradually, where 1 - object is fully hidden and 0 - object is fully shown
        /// </summary>
        public void ChangeEffectGradually(float targetValue, float duration, Action onCompleted = null)
        {
            if (currentAnimation != null)
            {
                currentAnimation.Kill(true);
            }

            currentAnimation = DOTween.To(() => targetRenderer.material.GetFloat("_Progress"), x => targetRenderer.material.SetFloat("_Progress", x), targetValue, duration).SetEase(Ease.Linear).OnComplete(() => { onCompleted?.Invoke(); });
        }
    }
}