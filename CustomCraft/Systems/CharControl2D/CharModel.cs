using AnimationView;
using CoreUtility.Extensions;
using UnityEngine;

namespace CharControl2D {
    public class CharModel {
        // Utility Conditions
        internal bool CanDash => IsDashReady && IsDashReset && !IsDashing;
        internal bool CanJump => (JumpCoyoteTime > 0f && CurrentJumpCount == 0 && CtrData.Jump.Count > 0) || (CurrentJumpCount < CtrData.Jump.Count && CurrentJumpCount != 0);
        internal bool CutJumpCondition => JumpProgressTime >= CtrData.Jump.MinCutPercent &&
                                         JumpProgressTime <= CtrData.Jump.MaxCutPercent;
        //internal bool CanMove => IsGrounded && !IsDashing && !IsJumping && AnimationService.IsPlaying(ViewData.Locomotion, 0);
        internal bool CanMove => AnimationService.IsPlaying(ViewData.Locomotion, 0);
        internal bool CanRoll => !IsRolling && IsGrounded && TimeToRoll <= 0 
                                 && AnimationExecutionValidator.IsTrue(ViewData.RollForward.Name)
                                 && AnimationExecutionValidator.IsTrue(ViewData.RollBackward.Name);
        internal bool CanFlip => (int)DirX != CurrentFlipDir;
        internal float SpeedByDir => Mathf.Approximately(DirX, -1) ? CtrData.Movement.BackwardMaxSpeed : CtrData.Movement.MaxSpeed;
        internal float AccelByDir => DirX == 0 ? CtrData.Movement.Deceleration : CtrData.Movement.Acceleration;
        
        // Collisions
        internal bool IsGrounded;
        internal bool IsCelling;
        internal int WallDir;
        internal bool IsWalling => WallDir != 0;
        internal bool IsColliding => IsGrounded || IsCelling || IsWalling;
        
        // Movement
        internal bool IsMovementBlocked;
        
        // Dash
        internal bool IsDashReset;
        internal bool IsDashReady;
        internal bool IsJumpCut;
        internal bool IsDashing;
        internal bool IsCrouch;

        // Jump
        internal bool IsJumping;
        internal bool IsFalling;
        internal float JumpForce;
        
        internal float JumpProgressTime;
        internal float JumpCoyoteTime;
        internal int CurrentJumpCount;
        
        // Roll
        internal float TimeToRoll;
        internal bool IsRolling;
        
        // Flip
        internal float SmoothedDirX;
        
        // Input
        internal float DirX;
        internal int RoundedDirX => (int)Mathf.Round(DirX);
        
        // Flip
        internal float LockedDirection => CtrData.Direction switch {
            FlipDirection.OnlyLeft => -1,
            FlipDirection.OnlyRight => 1f,
            _ => DirX.Without(0, CurrentFlipDir)
        };
        
        internal float LocomotionDir => (DirX * (int)CtrData.Direction).
            Without(0, Mathf.Abs(DirX));
        
        internal int CurrentFlipDir = 1;
        
        // Gravity
        internal float GravityStrength;
        internal float GravityScale;
        internal float GravityFall;
        internal float GravityJumpCutFall;
        
        internal CharControllerData CtrData;
        internal CharViewData ViewData;
        
        internal CharModel(CharControllerData ctrData, CharViewData viewData) {
            CtrData = ctrData;
            ViewData = viewData;

            InitializeData();
            CalculateDynamicData();
            
            // Dynamic data change
#if UNITY_EDITOR
            ctrData.OnValueChanged -= CalculateDynamicData;
            ctrData.OnValueChanged += CalculateDynamicData;
#endif
        }

        void InitializeData() {
            IsDashReset = true;
            IsDashReady = true;
            IsJumpCut = false;
        }

        void CalculateDynamicData() {
            JumpData jump = CtrData.Jump;
            
            GravityStrength = -(2 * jump.Height) / (jump.TimeToApex * jump.TimeToApex);
            GravityScale = GravityStrength / Physics2D.gravity.y;
            GravityFall = GravityScale * jump.Fall;
            GravityJumpCutFall = GravityScale * jump.CutFall;

            JumpForce = Mathf.Sqrt(2 * jump.Height * Mathf.Abs(GravityStrength));
            
            AnimationService.SetFloat(ViewData.RollForward.SpeedParameterName, CtrData.Roll.ParameterSpeed);
            AnimationService.SetFloat(ViewData.RollBackward.SpeedParameterName, CtrData.Roll.ParameterSpeed);
        }
    }
}