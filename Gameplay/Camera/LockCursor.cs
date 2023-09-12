using UnityEngine;


namespace GameZone.Scripts.Camera
{
    public class LockCursor : MonoBehaviour
    {
        private void Start() {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
