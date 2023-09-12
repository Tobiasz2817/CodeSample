using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameZone.Scripts.Animation
{
    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private List<AnimationUnit> units = new List<AnimationUnit>();
        [SerializeField] private bool isGettingReferencesFromChild = true;
    
        private bool processingHit = false;

        private AnimationUnit currentInvokeAnim;
        private void Awake() {
            if (isGettingReferencesFromChild)
                GetAttackReferencesFromChild();
        }

        private void OnEnable() {
            SubscribeEventsFromList();
        }

        private void OnDisable() {
            UnSubscribeEventsFromList();
        }
        
        private void GetAttackReferencesFromChild() {
            units.Clear();
            foreach (var attack in GetComponentsInChildren<AnimationUnit>()) {
                units.Add(attack);
            }
        }
        
        private void SubscribeEventsFromList() {
            foreach (var attacking in units) {
                attacking.OnPressedBind += DoSomething;
            }
        }
        
        private void UnSubscribeEventsFromList() {
            foreach (var attacking in units) {
                attacking.OnPressedBind -= DoSomething;
            }
        }

        private void DoSomething(AnimationUnit unit) {
            if (IsSomeAttackInvoke()) {
                return;
            }
       
            unit.MakingAttack();
            currentInvokeAnim = unit;
        }

        public bool IsSomeAttackInvoke() {
            foreach (var attack in units) 
                if (attack.IsAnimating()) 
                    return true;

            return false;
        }
    
        public bool IsCurrentAnimationBlockMovement() {
            foreach (var attack in units) 
                if (attack.IsAnimating()) 
                    if(attack.BlockingMovement())
                        return true;

            return false;
        }

    }

}

