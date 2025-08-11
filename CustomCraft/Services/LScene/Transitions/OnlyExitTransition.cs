using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

namespace LScene {
    public class OnlyExitTransition : TransitionView
    {
        [Header("Settings")]
        [SerializeField] float _closeDuration = 1f;
        [SerializeField] Ease _closeEase = Ease.OutQuad;
        [SerializeField] Image _closeImage;
        
        protected override IEnumerator OpenTransition() {
            yield return null;
        }
    
        protected override IEnumerator CloseTransition() {
            Vector3 range = new Vector3(0f, 0f, 0f);
            yield return _closeImage.transform.
                DOScale(range, _closeDuration).
                SetEase(_closeEase).
                WaitForCompletion();
        }
    }
}
