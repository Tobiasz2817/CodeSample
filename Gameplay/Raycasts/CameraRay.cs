using UnityEngine.InputSystem;
using GameZone.Scripts.Input;
using UnityEngine;
using Utilities;
using System;


namespace GameZone.Scripts.Raycasts
{
    public class CameraRay : Singleton<CameraRay>
    {
        [SerializeField] private LayerMask rayMask;
        [SerializeField] private float basedMaxDistance = 1000;
        [SerializeField] private float baseDistanceAboveGround = 0;
        [SerializeField] private bool displayRay = true;

        private Vector3 mousePointOnGround = Vector3.zero;
        private Vector2 mouseLastInput;

        public static event Action<RaycastHit> OnDetected;

        private InputActionReference inputRayReference;

        public override void Awake() {
            base.Awake();
            inputRayReference = InputManager.Instance.GetInput(InputManager.InputActionKey.MousePosition);
        }

        private void OnEnable() {
            inputRayReference.action.performed += ReadMousePosition;
            inputRayReference.action.Enable();
        }

        private void OnDisable() {
            inputRayReference.action.performed -= ReadMousePosition;
            inputRayReference.action.Disable();
        }

        private void ReadMousePosition(InputAction.CallbackContext obj) {
            mouseLastInput = obj.ReadValue<Vector2>();
            var raycast = MakeRay(mouseLastInput,rayMask, baseDistanceAboveGround,basedMaxDistance);
            mousePointOnGround = raycast.point;
            OnDetected?.Invoke(raycast);
        }
        
        public RaycastHit GetRay() {
            return MakeRay(mouseLastInput,rayMask,baseDistanceAboveGround,basedMaxDistance);
        }

        public RaycastHit GetRay(Vector3 pos,LayerMask layerMask, float distanceAboveGround = 0,float maxDistance = 1000) {
            return MakeRay(pos,layerMask,distanceAboveGround,maxDistance);
        }

        private RaycastHit MakeRay(Vector3 pos,LayerMask layerMask, float distanceAboveGround, float maxDistance) {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(pos);

            RaycastHit info;
            if (Physics.Raycast(ray, out info,maxDistance,layerMask)) {
                Vector3 planeNormal = Vector3.up; 
                Vector3 planePoint = new Vector3(info.point.x, info.point.y + distanceAboveGround, info.point.z);

                float d = Vector3.Dot(planeNormal, planePoint - ray.origin) / Vector3.Dot(planeNormal, ray.direction);

                Vector3 intersectionPoint = ray.origin + ray.direction * d;
                
                info.point = intersectionPoint;
            }

            return info;
        }

        private void Update() {
            if (!displayRay) return;
            Debug.DrawLine(UnityEngine.Camera.main.transform.position,mousePointOnGround,Color.black);
        }
    }

}
