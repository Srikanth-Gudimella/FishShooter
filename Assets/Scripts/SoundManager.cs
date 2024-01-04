using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FishShooting
{
    public class SoundManager : GenericSingleton<SoundManager>
    {
        public AudioSource m_BGAudioSource,m_AudioSource;
        public AudioClip[] m_BGClips;
        public AudioClip[] m_Clips;

        public override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            m_BGAudioSource.clip = m_BGClips[Random.Range(0, m_BGClips.Length)];
            m_BGAudioSource.Play();
        }

        public void PlaySound(int val)
        {
            m_AudioSource.clip = m_Clips[val];
            m_AudioSource.Play();
        }
        
    }
}
