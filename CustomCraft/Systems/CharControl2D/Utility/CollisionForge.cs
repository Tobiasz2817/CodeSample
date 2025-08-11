using CoreUtility.Extensions;
using CoreUtility;
using UnityEngine;
using System;

namespace CharControl2D {
    public class CollisionForge {
        internal const int CollisionCount = 10;
        
        internal readonly RaycastHit2D[] Hits;
        
        public Action<RaycastHit2D, bool, bool, int> OnCollisionEntry;
        /// <summary>
        /// It's reversion data, means true will be when something is out
        /// We're leaving IsGrounded so IsGrounded will be true but in real case will be false because we're leaving from that state
        /// </summary>
        public Action<bool, bool, int> OnCollisionExit;
        public Action<bool, bool, int> OnCollisionStay;
        
        public bool IsColliding => IsGrounded || IsWalling || IsCelling;
        public bool IsGrounded;
        public bool IsWalling => WallDir != 0;
        public bool IsCelling;
        public int WallDir;
        
        internal int HitCount;
        
        public bool WasGroundedThisFrame;
        
        BoxCollider2D _col2D;
        float _sizeOffset;
        LayerMask _collisionMask;

        public CollisionForge(BoxCollider2D collider2D, LayerMask collideLayerMask, int hitsMaxCount = 0, float sizeOffset = 0.1f) {
            _col2D = collider2D;
            _sizeOffset = sizeOffset;
            _collisionMask = collideLayerMask;
            
            hitsMaxCount = hitsMaxCount == 0 ? CollisionForge.CollisionCount : hitsMaxCount;
            
            Hits = new RaycastHit2D[hitsMaxCount];
        }
        
        public void Tick() {
            FetchCollisions();
            
            RemoveSelfCollision();
            ProcessOnEnter();
        }

        void ProcessOnEnter() {
            var states = (false, 0, false);
            
            bool prevIsGrounded = IsGrounded;
            bool prevIsCelling = IsCelling;
            int prevWallDir = WallDir;
            
            for (var i = 0; i < HitCount; i++) {
                var hit = Hits[i];

                var prevState = (IsGrounded, WallDir, IsCelling);
                var contacts = Utility.GetSurfaceContacts(hit.normal);
                // Can upgrade state when new state is true and before was false
                states.Item1 = states.Item1 || contacts.Item1;
                states.Item2 = states.Item2 != 0 ? states.Item2 : contacts.Item2;
                states.Item3 = states.Item3 || contacts.Item3;
                
                IsGrounded = contacts.Item1 || IsGrounded;
                WallDir = contacts.Item2 != 0 ? contacts.Item2 : WallDir;
                IsCelling = contacts.Item3 || IsCelling;
                
                // Notify entry/exit
                if (IsGrounded && !prevState.IsGrounded || 
                    IsCelling && !prevState.IsCelling || 
                    WallDir != 0 && prevState.WallDir == 0)
                    
                    OnCollisionEntry?.Invoke(hit, contacts.Item1, contacts.Item3, contacts.Item2);
            }
            
            if (prevIsGrounded && !states.Item1 || 
                prevIsCelling && !states.Item3 || 
                prevWallDir != 0 && states.Item2 == 0)
                OnCollisionExit?.Invoke(prevIsGrounded, prevIsCelling, prevWallDir);
            
            OnCollisionStay?.Invoke(states.Item1, states.Item3, states.Item2);
            
            IsGrounded = states.Item1;
            WallDir = states.Item2;
            IsCelling = states.Item3;
        }
        
        void RemoveSelfCollision() {
            int validHitCount = 0;
            for (int i = 0; i < HitCount; i++) {
                if (Hits[i].transform == _col2D.transform) 
                    continue;
                
                Hits[validHitCount] = Hits[i];
                validHitCount++;
            }

            Hits[validHitCount] = default;
            
            for (var i = HitCount; i < Hits.Length; i++) 
                Hits[i] = default;
        }

        void FetchCollisions() =>
            HitCount = Physics2D.BoxCastNonAlloc(_col2D.bounds.center, _col2D.bounds.size.AddAll(_sizeOffset),
                _col2D.transform.eulerAngles.z, Vector2.zero, Hits, float.MinValue, _collisionMask);
        
        public void OnDrawGizmos() {
            if (_col2D == null) 
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_col2D.bounds.center, _col2D.bounds.size);

            for (var i = 0; i < HitCount; i++) {
                if (Hits[i].collider == null || Hits[i].transform.Equals(_col2D.transform)) 
                    continue;
                
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(Hits[i].point, 0.1f);
            }
        }
        public override string ToString() =>
            "G: " + IsGrounded + " W: " + IsWalling + " " + WallDir + " C: " + IsCelling;
    }
}
