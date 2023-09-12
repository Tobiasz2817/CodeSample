using System.Collections;
using UnityEngine;
using System;

namespace GameZone.Scripts.Animation
{
    public abstract class AnimationUnit : MonoBehaviour
    {
        public Action<AnimationUnit> OnPressedBind;
        public Action<AnimationUnit> OnReleasedBind;
        public Action<AnimationUnit> OnTriggerEvent;
        public Action<AnimationUnit> OnStartAnim;
        public Action<AnimationUnit> OnExecuteAnim;
        protected bool isAnimating = false;

        [field:SerializeField] public float startMotionSpeed { private set; get; } = 1f;
        [field: SerializeField] public string speedAnimationFloat { private set; get; } = "AnimationSpeed";
        
        [SerializeField] protected AnimationInput animationInput;
        [SerializeField] protected Animator animator;

        protected virtual void OnEnable() {
            if (animationInput == null) return;
            animationInput.OnPress += InvokePressBind;
            animationInput.OnRealesed += InvokeReleasedBind;
        }

        protected virtual void OnDisable() {
            if (animationInput == null) return;
            animationInput.OnPress -= InvokePressBind;
            animationInput.OnRealesed -= InvokeReleasedBind;
        }
        
        
        protected virtual void InvokePressBind() {
            OnPressedBind?.Invoke(this);
        }
        
        protected virtual void InvokeReleasedBind() {
            OnReleasedBind?.Invoke(this);
        }
        
        public void AnimTriggerHandler() {
            if (!IsAnimating()) 
                return;
            OnTriggerEvent?.Invoke(this);
        }

        protected void SetAnimationSpeed(string nameParameterSpeed,float newSpeed) {
            animator.SetFloat(nameParameterSpeed,newSpeed);
        }
        
        protected IEnumerator StopBeforeSomeTime(float breakTimeCombo) {
            yield return new WaitUntil(() => {
                var animInfo = animator.GetCurrentAnimatorStateInfo(0);
                var stop = animInfo.length - (animInfo.length * breakTimeCombo);
                var currentAnimFrame = animInfo.normalizedTime * animInfo.length;
                return currentAnimFrame > stop;
            });
        }
        
        public IEnumerator StopBeforeAnimReachTime(float animationPercent) {
            yield return new WaitUntil(() => {
                var animInfo = animator.GetCurrentAnimatorStateInfo(0);
                var stop = animInfo.length * Mathf.Clamp(animationPercent / 100,0f,0.99f);
                var currentAnimFrame = animInfo.normalizedTime * animInfo.length;
                return currentAnimFrame > stop;
            });
        }
        
        protected IEnumerator WaitForEndAnimation(string nameAnimation) {
            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName(nameAnimation));
        }
        
        public abstract void MakingAttack();
        public abstract bool IsAnimating();
        public abstract bool BlockingMovement();
        public abstract float GetAnimationSpeed();
        public abstract AnimationClip GetCurrentAnimationClip();
    }
}