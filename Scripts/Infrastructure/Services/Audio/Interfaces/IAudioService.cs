using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.Audio
{
    public interface IAudioService
    {
        public void ChangeSoundVolume(float volume);
        public void ChangeMusicVolume(float volume);
        public void ChangeSoundState(bool state);
        public void ChangeMusicState(bool state);
        public float GetSoundDuration(string key);
        public void PlaySound(string key);
        public UniTask PlaySound(string key, float delay);
        public void PlayMusic(string key);
        public void StopMusic();
        public bool IsSoundsOn { get; }
        public bool IsMusicOn { get; }
        public float SoundsVolume { get; }
        public float MusicVolume { get; }
    }
}
