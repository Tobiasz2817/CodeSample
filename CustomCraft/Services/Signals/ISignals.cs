

using System;

namespace Signals {
    // Must be prefix "On"
    public interface ISignals {
        void OnMovementCondition(bool condition);
        void OnDamageDeal(float damage);
        void OnCharacterJump(int jumpCount);
        void OnTakeDamage(float damage, int state);
        void PlayerDead();
        void SendRPC();
        void PlayerRespawn();
        void SubscribeEvent(int enumId, Action callback);
    }
}