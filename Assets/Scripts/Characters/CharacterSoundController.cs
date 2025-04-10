using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Projectiles.Characters
{
    /// <summary>
    /// Used to control game characters' sound effect
    /// </summary>

    public class CharacterSoundController : MonoBehaviour
    {
        [SerializeField]
        AudioSource effectSource;

        [SerializeField]
        AudioClip throwSound, hitSound, deadSound;


        public void PlayHitSound()
        {
            if (hitSound != null && effectSource != null)
            {
                effectSource.PlayOneShot(hitSound);
            }
        }

        public void PlayThrowSound()
        {
            if (throwSound != null && effectSource != null)
            {
                effectSource.PlayOneShot(throwSound);
            }
        }

        public void PlayDeadSound()
        {
            if (deadSound != null && effectSource != null)
            {
                effectSource.PlayOneShot(deadSound);
            }
        }
    }
}