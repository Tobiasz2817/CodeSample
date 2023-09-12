using System;
using UnityEngine;


namespace GameZone.Multiplayer.Game.Bullet
{
    public class BasicBulletSinglePlayer : BulletBase
    {
        protected override void OnEnable() { }
        protected override void OnDisable() { }

        protected override void Update() {
            UpdatePosition(transform.forward);
        }
    
        protected override void UpdatePosition(Vector3 position) {
            transform.position += position * bulletSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other) {
            Destroy(gameObject);
        }
    }
}
