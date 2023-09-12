using UnityEngine;
using System;

namespace GameZone.Scripts.Animation
{
    public abstract class AnimationInput : MonoBehaviour
    {
        public Action OnPress;
        public Action OnRealesed;
    }
}