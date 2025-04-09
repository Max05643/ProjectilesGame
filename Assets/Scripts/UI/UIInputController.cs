using System.Collections;
using System.Collections.Generic;
using Projectiles.Characters;
using UnityEngine;
using UnityEngine.UI;
using TouchControlsKit;
using Zenject;

namespace Projectiles.UI
{

    /// <summary>
    /// Controls input from UI elements
    /// </summary>
    public class UIInputController : MonoBehaviour
    {
        [Inject]
        PlayerController playerController;


        bool aiming = false;

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
                    playerController.StopPreparingAttack();
                }
                aiming = false;
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