using AtomicApps.Infrastructure.Services.Audio.Data;
using AtomicApps.Infrastructure.Services.SaveLoad;
using AtomicApps.Tools;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace AtomicApps.Infrastructure.Services.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [HelpBox(
            @"SETUP INSTRUCTION
1. Add to collection below all sounds you want to play.
2. In AudioCollection ScriptableObject press button Generate Constants. It will add all sound id's to a file SoundKeys.
3. Call PlaySound or PlayMusic methods with id's from SoundKeys to play VFX.", HelpBoxMessageType.Info)]
        [Space]
        [SerializeField]
        private AudioProvider _audioProvider;
        [SerializeField]
        private AudioCollection _collection;
        
        private AudioMixer _mixer;
        private ISaveService _saveService;
        private AudioSaveData _audioSaveData;
        private string _currentMusicKey;

        public bool IsSoundsOn => _audioSaveData.IsSoundOn;

        public bool IsMusicOn => _audioSaveData.IsMusicOn;
        public float SoundsVolume => _audioSaveData.SoundsVolume;
        public float MusicVolume => _audioSaveData.MusicVolume;

        [Inject]
        private void Construct(ISaveService saveService)
        {
            _saveService = saveService;
            
            Initialize();
        }

        private void Initialize()
        {
            _mixer = _audioProvider.Mixer;

            _audioSaveData = _saveService.GetData<AudioSaveData>();

            _audioProvider.SfxSource.mute = !_audioSaveData.IsSoundOn;
            _audioProvider.MusicSource.mute = !_audioSaveData.IsMusicOn;
            
            _audioProvider.SfxSource.volume = _audioSaveData.SoundsVolume;
            _audioProvider.MusicSource.volume = _audioSaveData.MusicVolume;
        }

        public void ChangeSoundVolume(float volume)
        {
            _audioSaveData.SoundsVolume = volume;
            
            _audioProvider.SfxSource.volume = volume;
            
            _saveService.SaveDataImmediately(_audioSaveData);
        }

        public void ChangeMusicVolume(float volume)
        {
            _audioSaveData.MusicVolume = volume;
            
            _audioProvider.MusicSource.volume = volume;
            
            _saveService.SaveDataImmediately(_audioSaveData);
        }

        public void ChangeSoundState(bool state)
        {
            _audioSaveData.IsSoundOn = state;

            _audioProvider.SfxSource.mute = !state;
            
            _saveService.SaveDataImmediately(_audioSaveData);
        }

        public void ChangeMusicState(bool state)
        {
            _audioSaveData.IsMusicOn = state;
            
            _audioProvider.MusicSource.mute = !state;
            
            _saveService.SaveDataImmediately(_audioSaveData);
        }

        public float GetSoundDuration(string key)
        {
            var clip = _collection.GetMappingByKey(key);
            return clip.Clip.length;
        }

        private void Play(SoundMapping mapping)
        {
            if (mapping == null)
            {
                Debug.LogWarning("Audio clip is null");
                return;
            }

            _audioProvider.SfxSource.pitch = 1f + Random.Range(-mapping.pitchRandomisation, mapping.pitchRandomisation);
            _audioProvider.SfxSource.PlayOneShot(mapping.Clip);
        }

        public void PlaySound(string key)
        {
            if(!IsSoundsOn) return;

            var clip = _collection.GetMappingByKey(key);
            Play(clip);
        }
        
        public async UniTask PlaySound(string key, float delay)
        {
            if(!IsSoundsOn) return;
            
            await UniTask.WaitForSeconds(delay);

            var clip = _collection.GetMappingByKey(key);
            Play(clip);
        }

        public void PlayMusic(string key)
        {
            if(!IsMusicOn || _currentMusicKey == key) return;

            var mapping = _collection.GetMappingByKey(key);

            if (mapping == null)
            {
                Debug.LogWarning("Audio clip is null");
                return;
            }
            
            _audioProvider.MusicSource.Stop();
            _audioProvider.MusicSource.clip = mapping.Clip;
            _audioProvider.MusicSource.Play();
            _currentMusicKey = key;
        }

        public void StopMusic()
        {
            _audioProvider.MusicSource.Stop();
        }
    }
}