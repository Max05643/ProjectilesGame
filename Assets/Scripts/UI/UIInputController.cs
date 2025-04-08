using System.Collections;
using System.Collections.Generic;
using Projectiles.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Projectiles.UI
{

    /// <summary>
    /// Controls input from UI elements
    /// </summary>
    public class UIInputController : MonoBehaviour
    {
        [SerializeField]
        PlayerController playerController;

        [SerializeField]
        Slider horizontalProjectileAngleSlider, verticalProjectileAngleSlider;

        [SerializeField]
        Toggle attackPrepareToggle;

        [SerializeField]
        Button attackButton;

        [SerializeField]
        Slider xAxisSlider, yAxisSlider;

        void Start()
        {
            attackPrepareToggle.onValueChanged.AddListener(OnAttackPrepareToggleChanged);
            attackButton.onClick.AddListener(OnAttackButtonClicked);
            horizontalProjectileAngleSlider.onValueChanged.AddListener(OnHorizontalProjectileAngleSliderChanged);
            verticalProjectileAngleSlider.onValueChanged.AddListener(OnVerticalProjectileAngleSliderChanged);
            xAxisSlider.onValueChanged.AddListener(OnXAxisSliderChanged);
            yAxisSlider.onValueChanged.AddListener(OnYAxisSliderChanged);


            Repaint();
        }

        void Repaint()
        {
            attackButton.gameObject.SetActive(playerController.IsAttackPrepared);
            horizontalProjectileAngleSlider.gameObject.SetActive(playerController.IsAttackPrepared);
            verticalProjectileAngleSlider.gameObject.SetActive(playerController.IsAttackPrepared);
            attackPrepareToggle.SetIsOnWithoutNotify(playerController.IsAttackPrepared);

            if (playerController.IsAttackPrepared)
            {
                horizontalProjectileAngleSlider.normalizedValue = 0.5f;
                verticalProjectileAngleSlider.normalizedValue = 0.5f;
                OnVerticalProjectileAngleSliderChanged(0.5f);
                OnHorizontalProjectileAngleSliderChanged(0.5f);
            }
        }

        void OnAttackPrepareToggleChanged(bool isOn)
        {
            if (isOn)
            {
                playerController.StartPreparingAttack();
            }
            else
            {
                playerController.StopPreparingAttack();
            }

            Repaint();
        }

        void OnAttackButtonClicked()
        {
            playerController.Fire();
        }

        void OnHorizontalProjectileAngleSliderChanged(float value)
        {
            playerController.SetNormalizedHorizontalProjectileAngle(value);
        }

        void OnVerticalProjectileAngleSliderChanged(float value)
        {
            playerController.SetNormalizedVerticalProjectileAngle(value);
        }

        void OnXAxisSliderChanged(float value)
        {
            playerController.SetXAxisNormalized(value);
        }

        void OnYAxisSliderChanged(float value)
        {
            playerController.SetYAxisNormalized(value);
        }


    }
}