using System.Collections;
using UnityEngine;
using System;

namespace GameZone.Scripts.Animation
{
    public class AnimationSingleUnit : AnimationUnit
    {
        [Serializable]
        public class AnimationSingleDependencies
        {
            [field:SerializeField] public float damage { private set; get; }
            [field:SerializeField] public float breakTime { private set; get; }
            [field:SerializeField] public float incrementSpeedAnim { set; get; } = 0.2f;
            [field:SerializeField] public AnimationClip animationClip { private set; get; }
        }

        [SerializeField] private AnimationSingleDependencies singleAnimation;

        private void Start() {
            var speed = startMotionSpeed + (startMotionSpeed * singleAnimation.incrementSpeedAnim);
            singleAnimation.incrementSpeedAnim = speed;
        }

        public override void MakingAttack() {
            StartCoroutine(MakingCombo());
        }
        
        public override AnimationClip GetCurrentAnimationClip() {
            return singleAnimation.animationClip;
        }

        public IEnumerator MakingCombo() {
            isAnimating = true;
            animator.Play(singleAnimation.animationClip.name);
            OnStartAnim?.Invoke(this);
            SetAnimationSpeed(speedAnimationFloat,singleAnimation.incrementSpeedAnim);
            yield return new WaitForSeconds(0.02f);
            yield return StopBeforeSomeTime(singleAnimation.breakTime);
            isAnimating = false;
            SetAnimationSpeed(speedAnimationFloat,startMotionSpeed);
            OnExecuteAnim?.Invoke(this);
        }
        
        
        public float GetBreakTime() {
            return singleAnimation.breakTime;
        }

        public override bool IsAnimating() {
            return isAnimating;
        }

        public override bool BlockingMovement() {
            return false;
        }

        public override float GetAnimationSpeed() {
            return startMotionSpeed;
        }
    }
}