using System.Collections;
using System.Collections.Generic;
using Projectiles.Characters;
using UnityEngine;
using UnityEngine.UI;
using TouchControlsKit;
using Zenject;
using Projectiles.Music;

namespace Projectiles.UI
{

    /// <summary>
    /// Controls input from UI elements
    /// </summary>
    public class UIInputController : MonoBehaviour
    {
        [Inject]
        PlayerController playerController;

        [SerializeField]
        HealthBarController healthBarController;

        [SerializeField]
        Button reviveButton;

        [SerializeField]
        GameObject deathScreen;

        [SerializeField]
        Button muteButton, unMuteButton;

        [Inject]
        MusicController musicController;

        bool aiming = false;

        void MuteStatusChange()
        {
            musicController.Muted = !musicController.Muted;

            muteButton.gameObject.SetActive(!musicController.Muted);
            unMuteButton.gameObject.SetActive(musicController.Muted);
        }

        void Start()
        {
            playerController.onDeath.AddListener(OnDeath);
            playerController.onHealthChanged.AddListener(OnHealthChanged);
            healthBarController.SetNormalizedHealth(playerController.HealthNormalized, false);
            reviveButton.onClick.AddListener(() =>
            {
                playerController.Revive();
            });
            deathScreen.SetActive(false);
            muteButton.onClick.AddListener(() =>
            {
                MuteStatusChange();
            });
            unMuteButton.onClick.AddListener(() =>
            {
                MuteStatusChange();
            });
        }

        void OnHealthChanged()
        {
            float healthNormalized = playerController.HealthNormalized;
            healthBarController.SetNormalizedHealth(playerController.HealthNormalized, true);

            deathScreen.SetActive(healthNormalized <= 0);
        }

        void OnDeath()
        {
            healthBarController.SetNormalizedHealth(0, true);
        }

        float MapFromInputToNormalized(float value)
        {
            return (value + 1f) / 2f;
        }

        void Update()
        {
            float xMovement = MapFromInputToNormalized(TCKInput.GetAxis("MovementJoystick", EAxisType.Horizontal));
            float yMovement = MapFromInputToNormalized(TCKInput.GetAxis("MovementJoystick", EAxisType.Vertical));

            float xAim = MapFromInputToNormalized(TCKInput.GetAxis("AimJoystick", EAxisType.Horizontal));
            float yAim = MapFromInputToNormalized(TCKInput.GetAxis("AimJoystick", EAxisType.Vertical));

            var aimPhase = TCKInput.GetTouchPhase("AimJoystick");

            playerController.SetXAxisNormalized(xMovement);
            playerController.SetYAxisNormalized(yMovement);




            if (aimPhase == ETouchPhase.NoTouch || aimPhase == ETouchPhase.Ended)
            {
                if (aiming && playerController.IsAttackPrepared)
                {
                    playerController.Fire();
                }
                aiming = false;
                playerController.StopPreparingAttack();
            }
            else
            {
                aiming = true;
                playerController.StartPreparingAttack();
                playerController.SetNormalizedHorizontalProjectileAngle(xAim);
                playerController.SetNormalizedVerticalProjectileAngle(yAim);
            }


            TCKInput.SetControllerActive("AimJoystick", playerController.CanPrepareToAttackAgain);
            TCKInput.SetControllerActive("MovementJoystick", playerController.CanMove);
        }
    }
}