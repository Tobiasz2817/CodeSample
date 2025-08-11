using System;
using UnityEngine;

namespace CharControl2D {
    [Serializable]
    public struct MovementData {
        public float MaxSpeed;
        public float BackwardMaxSpeed;
        [Range(0.01f, 1)] public float Acceleration;
        [Range(0.01f, 1)] public float Deceleration;
        [Range(0.01f, 1)] public float FallingAcceleration;
        [Range(0.01f, 1)] public float FlyingAcceleration;

        public static MovementData Default => new() {
            MaxSpeed = 11f,
            BackwardMaxSpeed = 8f,
            Acceleration = 1f,
            Deceleration = 1f,
            FallingAcceleration = 0.4f,
            FlyingAcceleration = 0.4f
        };
    }
    
    [Serializable]
    public struct JumpData {
        public int Count;
        public float Height;
        public float TimeToApex;
        public float Fall;
        public float CutFall;
        [Range(0, 1)] public float MinCutPercent;
        [Range(0, 1)] public float MaxCutPercent;
        public float CoyoteTime;
        public float InputBuffer;

        public static JumpData Default => new() {
            Count = 2,
            Height = 3f,
            TimeToApex = 0.3f,
            Fall = 1f,
            CutFall = 1.25f,
            MinCutPercent = 0f,
            MaxCutPercent = 0.8f,
            CoyoteTime = 0.05f,
            InputBuffer = 0.2f,
        };
    }
    
    [Serializable]
    public struct RollData {
        [Header("Settings")]
        public float InputBuffer;
        public float ParameterSpeed;
        public float Cooldown;
        [Header("Forward")]
        public float DurationForward;
        public float DistanceForward;
        [Header("Backward")]
        public float DurationBackward;
        public float DistanceBackward;
        
        public static RollData SlowRoll => new() {
            Cooldown = 0.2f,
            DurationForward = 2f,
            DistanceForward = 0.5f,
            DurationBackward = 2f,
            DistanceBackward = 0.5f,
            InputBuffer = 0.1f,
            ParameterSpeed = 1f,
        };
    }
    
    [Serializable]
    public struct DashData {
        public float Cooldown;
        public float Speed;
        public float Duration;
        public float InputBuffer;
        public float GravityFall;
        public AnimationCurve Curve;

        public static DashData Default => new() {
            Cooldown = 0.2f,
            Speed = 20f,
            Duration = 0.2f,
            InputBuffer = 0.1f,
            GravityFall = 0f,
            Curve = AnimationCurve.Constant(0f, 1f, 1f)
        };
    }
    
    [Serializable]
    public struct WallSlideData {
        public float Fall;

        public static WallSlideData Default => new() {
            Fall = 1f
        };
    }
}