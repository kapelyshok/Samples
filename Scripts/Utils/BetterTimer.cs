using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AtomicApps
{
        public class BetterTimer
    {
        private float _offset;
        private float _time;
        private bool _loop;
        private readonly bool _playAwake;

        public event Action Updated;
        public event Action Completed;
        public event Action Playing;
        
        public bool IsCompleted { get; private set; }
        public bool IsPaused { get; private set; }
        public float CurrentValue { get; private set; }
        public float PreviousTime { get; private set; }

        public float MaxValue => _time;

        public BetterTimer()
        {
            IsCompleted = true;
        }

        public BetterTimer(float time, float offset = 0f, bool loop = false, bool playAwake = false)
        {
            _time = time;
            _offset = offset;
            PreviousTime = CurrentValue = _time + Random.Range(-_offset, _offset);
            _loop = loop;
            _playAwake = playAwake;

            if (!_playAwake)
            {
                IsCompleted = true;
                PreviousTime = CurrentValue = 0f;
            }
        }
        
        public void Pause()
        {
            IsPaused = true;
        }

        public void UnPause()
        {
            IsPaused = false;
        }

        public void AddToCurrentTime(float value)
        {
            CurrentValue += value;
            Tick();
        }

        public void SetTime(float time)
        {
            _time = time;
        }

        public void SetLoop(bool isLoop)
        {
            _loop = isLoop;
        }

        public void SetOffset(float offset)
        {
            _offset = offset;
        }

        public void Reset()
        {
            UnPause();
            CurrentValue = _time + Random.Range(-_offset, _offset);
            Playing?.Invoke();
            IsCompleted = false;
        }

        public void Tick()
        {
            if (IsCompleted || IsPaused)
            {
                return;
            }
            PreviousTime = CurrentValue;
            CurrentValue -= Time.deltaTime;

            Updated?.Invoke();

            if (CurrentValue > 0) return;
            CurrentValue = 0f;

            IsCompleted = true;

            Completed?.Invoke();

            if (!_loop) return;
            
            CurrentValue = _time + Random.Range(-_offset, _offset);
            IsCompleted = false;
            Playing?.Invoke();
            return;
        }
    }
}
