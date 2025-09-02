using System;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.Audio
{
    [Serializable]
    public class SoundMapping
    {
        public string Key;
        public AudioClip Clip;
        public float pitchRandomisation = 0f;
    }
}