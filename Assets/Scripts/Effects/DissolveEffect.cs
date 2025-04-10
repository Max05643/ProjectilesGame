using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Projectiles.Settings;
using UnityEngine;
using Zenject;

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

        [Inject]
        EffectsSettings effectsSettings;

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

        public void ShowGradually(Action onCompleted = null)
        {
            if (currentAnimation != null)
            {
                currentAnimation.Kill(true);
            }

            currentAnimation = DOTween.To(() => targetRenderer.material.GetFloat("_Progress"), x => targetRenderer.material.SetFloat("_Progress", x), 0, effectsSettings.timeForItemToAppear).SetEase(Ease.Linear).OnComplete(() => { onCompleted?.Invoke(); });
        }

        /// <summary>
        /// Hides the object gradually
        /// </summary>
        public void HideGradually(Action onCompleted = null)
        {
            if (currentAnimation != null)
            {
                currentAnimation.Kill(true);
            }

            currentAnimation = DOTween.To(() => targetRenderer.material.GetFloat("_Progress"), x => targetRenderer.material.SetFloat("_Progress", x), 1, effectsSettings.timeForItemToDisappear).SetEase(Ease.Linear).OnComplete(() => { onCompleted?.Invoke(); });
        }
    }
}