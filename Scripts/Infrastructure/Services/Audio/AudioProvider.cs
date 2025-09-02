using UnityEngine;
using UnityEngine.Audio;

namespace AtomicApps.Infrastructure.Services.Audio
{
    public class AudioProvider : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioMixer mixer;

        public AudioSource SfxSource => sfxSource;

        public AudioSource MusicSource => musicSource;

        public AudioMixer Mixer => mixer;
    }
}