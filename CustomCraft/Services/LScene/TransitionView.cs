using System.Collections;
using UnityEngine;
using TMPro;

namespace LScene {
    public abstract class TransitionView : MonoBehaviour {

        [SerializeField] internal TransitionType TransitionType = TransitionType.Default;
        [Space]
        [Space]
        [SerializeField] TMP_Text _statusText;

        public bool IsOpen { get; private set; }
        public bool IsClosed { get; private set; }

        public string StatusText {
            set => _statusText.text = value;
        }

        public void Open() => StartCoroutine(ProcessOpen());
        public void Close() => StartCoroutine(ProcessClose());

        IEnumerator ProcessOpen() {
            IsOpen = false; 
            yield return OpenTransition();
            IsOpen = true;
        }
        
        IEnumerator ProcessClose() {
            IsClosed = false;
            yield return CloseTransition();            
            IsClosed = true;
        }

        protected abstract IEnumerator OpenTransition();
        protected abstract IEnumerator CloseTransition();
    }

    public enum TransitionType {
        Default = 1,
        OnlyExit = 2,
    }
}