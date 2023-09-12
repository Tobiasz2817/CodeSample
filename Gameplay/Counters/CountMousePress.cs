using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameZone.Scripts.Counters
{
    public class CountMousePress
    {
        private Dictionary<int, float> pressTimer = new Dictionary<int, float>();

        private int currentTimePressed = 0;
    
        public void IncrementPressing() {
            currentTimePressed++;
        
            pressTimer.Add(currentTimePressed,Time.time);
        }

        public void ResetTime() {
            pressTimer.Clear();
            currentTimePressed = 0;
        }

        public bool ButtonWasPressedLastTime(float duration) {
            var currentTime = Time.time;

            if (pressTimer.Values.Count <= 0) return false;
            if (currentTime - pressTimer.Values.Last() < duration)
                return true;
        
            return false;
        }
    }

}
