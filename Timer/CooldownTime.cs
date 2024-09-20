using System;
using UnityEngine;

namespace Timer
{
    public class CooldownTime : ITimer
    {
        private float _cooldownTime = 2f;

        private float _minTime = 0f;
        private float _currentTime = 0f;

        private bool _isEnd = true;
        
        public Action OnCooldownStart;
        public Action OnCooldownEnd;

        public CooldownTime(float minTime = 0f, float cooldownTime = 2f) {
            this._minTime = minTime;
            this._cooldownTime = cooldownTime;

            _currentTime = _minTime;
        }

        public void StartTimer() {
            OnCooldownStart?.Invoke();
            _isEnd = false;
            _currentTime = _cooldownTime;
        }

        public void StopTimer() {
            _isEnd = true;
            _currentTime = _minTime;
        }

        public void TickTimer() {
            if (_isEnd) return;

            _currentTime -= Time.deltaTime;
            if (_currentTime > _minTime) return;
            
            _isEnd = true;
            OnCooldownEnd?.Invoke();
        }

        public bool IsFinish() => _currentTime <= _minTime;
        public bool IsEnd() => _currentTime <= _minTime;
        public float TimeToEnd() => _currentTime - _minTime;
    }

    public interface ITimer
    {
        void StartTimer();
        void StopTimer();
        void TickTimer();
        bool IsFinish();

        public static ITimer CreateDefault() => 
            new CooldownTime(0, 0.15f);
    }
}