using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UI.Popups {
    [RequireComponent(typeof(Button))]
    public class BindView : MonoBehaviour {
        [SerializeField] TMP_Text _bindText;
        [SerializeField] TMP_Text _rebindingText;
        [SerializeField] Image _bindImage;
        [SerializeField] Button _button;

        void OnValidate() => _button ??= GetComponent<Button>();

        internal void StartRebinding() {
            _rebindingText.gameObject.SetActive(true);
            _bindImage.gameObject.SetActive(false);
        }
        
        internal void StopRebinding() {
            _rebindingText.gameObject.SetActive(false);
            
            bool imageState = _bindImage.sprite;
            _bindImage.gameObject.SetActive(imageState);
        }
        
        internal void UpdateName(string bindName) => _bindText.text = bindName;

        internal void UpdateImage(Sprite bindImage) {
            _bindImage.sprite = bindImage;
            _bindImage.gameObject.SetActive(bindImage);
        }
        internal void AddBindingCallback(UnityAction callback) => _button.onClick.AddListener(callback);
    }
}