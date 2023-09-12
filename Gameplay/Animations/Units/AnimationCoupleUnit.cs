 using GameZone.Scripts.Counters;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

namespace GameZone.Scripts.Animation
{
    public class AnimationCoupleUnit : AnimationUnit
    {
        private Coroutine invokingCombo;
        private CountMousePress countMousePress;
        
        [Serializable]
        public class CoupleDependencies
        {
            
            [field: SerializeField] public float lastPressedBeforeTime = 0.2f;
            [field:SerializeField] public int priorty { private set; get; }
            [field:SerializeField] public float breakTime { private set; get; } = 0.2f;
            [field:SerializeField] public float incrementSpeedAnim { set; get; } = 0.2f;
            [field:SerializeField] public AnimationClip animationClip { private set; get; }
        }

        [SerializeField] private bool basedOnMousePressing = false;
        [SerializeField] protected List<CoupleDependencies> comboDependenciesList = new List<CoupleDependencies>();
        private CoupleDependencies currentDependencies;

        private void Start() {
            countMousePress = new CountMousePress();
            comboDependenciesList = comboDependenciesList.OrderBy((valeus) => valeus.priorty).ToList();

            comboDependenciesList.ForEach((unit) => {
                var speed = startMotionSpeed + (startMotionSpeed * unit.incrementSpeedAnim);
                unit.incrementSpeedAnim = speed;
            });
        }

        public override void MakingAttack() {
            if (isAnimating) return;
            StartCoroutine(MakingCombo());
            isAnimating = true;
        }

        public override bool IsAnimating() {
            return isAnimating;
        }

        public override bool BlockingMovement() {
            return false;
        }

        public override float GetAnimationSpeed() {
            return currentDependencies.incrementSpeedAnim;
        }

        public override AnimationClip GetCurrentAnimationClip() {
            return currentDependencies.animationClip;
        }

        private IEnumerator MakingCombo() {
            foreach (var attack in comboDependenciesList) {
                currentDependencies = attack;
                animator.Play(attack.animationClip.name);
                OnStartAnim?.Invoke(this);
                SetAnimationSpeed(speedAnimationFloat,attack.incrementSpeedAnim);
                yield return new WaitForSeconds(0.02f);
                yield return StopBeforeSomeTime(attack.breakTime);
                SetAnimationSpeed(speedAnimationFloat, startMotionSpeed);
                OnExecuteAnim?.Invoke(this);
                if (!basedOnMousePressing) continue;
                if (!countMousePress.ButtonWasPressedLastTime(attack.lastPressedBeforeTime)) break;
            }
            
            currentDependencies = null;
            countMousePress.ResetTime();
            isAnimating = false;
        }

        private void IncrementPress() {
            if (!basedOnMousePressing) return;
            countMousePress.IncrementPressing();
        }
        
        protected override void InvokePressBind() {
            base.InvokePressBind();
            IncrementPress();
        }

        public bool IsInvokeLastAnimation() {
            if (currentDependencies == null) return false;

            return currentDependencies == comboDependenciesList[^1];
        }
    }
}