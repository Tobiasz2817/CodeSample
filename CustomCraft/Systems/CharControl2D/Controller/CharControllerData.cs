using UnityEngine;
using System;

namespace CharControl2D
{
    [CreateAssetMenu(menuName = "CharControl/Data/Controller", fileName = "CharControllerData")]
    public class CharControllerData : ScriptableObject
    {
        [Header("Gravity")]
        [SerializeField] public float GravityMaxFall = 30f;
        
        [SerializeField]
        [Tooltip("Velocity fall threshold...")]
        public float VelocityFallThreshold = 0.5f;
        
        [Header("Flip settings")]
        [SerializeField] public float InputSmoothTime = 0.2f;
        
        [Header("Target Mode")]
        [SerializeField] public FlipDirection Direction = FlipDirection.LeftAndRight;
        
        [field: Header("Data")]
        [SerializeField] public MovementData Movement = MovementData.Default;
        [SerializeField] public JumpData Jump = JumpData.Default;
        [SerializeField] public DashData Dash = DashData.Default;
        [SerializeField] public RollData Roll = RollData.SlowRoll;
        [SerializeField] public WallSlideData WallSlide = WallSlideData.Default;

        internal event Action OnValueChanged;

        void OnValidate() => OnValueChanged?.Invoke();
    }
}