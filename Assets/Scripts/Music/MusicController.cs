using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles.Music
{
    /// <summary>
    /// Used to control the music in the game
    /// </summary>
    public class MusicController : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;


        bool muted = false;

        /// <summary>
        /// Whether the music is muted or not
        /// </summary>
        public bool Muted
        {
            get => muted;
            set
            {
                muted = value;
                audioSource.mute = value;
            }
        }

        void Start()
        {
            audioSource.Play();
        }
    }
}