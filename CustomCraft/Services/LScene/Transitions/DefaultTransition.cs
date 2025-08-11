using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LScene {
    public class DefaultTransition : TransitionView {
        [SerializeField] float _openRange = 4000f;
        [SerializeField] float _openDuration = 1f;
        [SerializeField] Ease _openEase = Ease.OutBounce;
        [SerializeField] float _closeRange = 0f;
        [SerializeField] float _closeDuration = 1f;
        [SerializeField] Ease _closeEase = Ease.OutBounce;
        [SerializeField] Image _circleImage;


        protected override IEnumerator OpenTransition() {
            Vector3 range = new Vector3(_openRange,_openRange,_openRange);
            yield return _circleImage.transform.
                DOScale(range, _openDuration).
                SetEase(_openEase).
                WaitForCompletion();
        }

        protected override IEnumerator CloseTransition() {
            Vector3 range = new Vector3(_closeRange,_closeRange,_closeRange);
            yield return _circleImage.transform.
                DOScale(range, _closeDuration).
                SetEase(_closeEase).
                WaitForCompletion();
        }
    }
}