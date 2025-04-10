using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Projectiles.UI
{
    /// <summary>
    /// Controls the health bar UI element
    /// </summary>
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField]
        RectTransform healthBarImage;

        [SerializeField]
        float minX, maxX;


        Tween currentAnimation = null;

        void OnDestroy()
        {
            currentAnimation?.Kill(true);
        }

        public void SetNormalizedHealth(float normalizedHealth, bool shouldAnimate)
        {
            currentAnimation?.Kill(true);


            float targetX = Mathf.Lerp(minX, maxX, normalizedHealth);


            if (shouldAnimate)
            {
                currentAnimation = DOTween.To(() => healthBarImage.sizeDelta.x, x => healthBarImage.sizeDelta = new Vector2(x, healthBarImage.sizeDelta.y), targetX, 0.5f).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    currentAnimation = null;
                });
            }
            else
            {
                healthBarImage.sizeDelta = new Vector2(targetX, healthBarImage.sizeDelta.y);
            }
        }
    }
}