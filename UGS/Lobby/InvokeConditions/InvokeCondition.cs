
namespace GameZone.Multiplayer.UGS.Lobby.InvokeCondition
{
    public class InvokeCondition
    {
        private readonly bool isBasedOnIsInvoke;
        private readonly bool isWaitUntilIsInvoke;
        private readonly bool isWaitUntilRateLimitDown;

        public InvokeCondition() : this(true, false, false) {
        }

        public InvokeCondition(bool IsBasedOnIsInvoke, bool IsWaitUntilIsInvoke, bool IsWaitUntilRateLimitDown) {
            this.isBasedOnIsInvoke = IsBasedOnIsInvoke;
            this.isWaitUntilIsInvoke = IsWaitUntilIsInvoke;
            this.isWaitUntilRateLimitDown = IsWaitUntilRateLimitDown;
        }

        public bool IsBasedOnIsInvoke() => isBasedOnIsInvoke;
        public bool IsWaitUntilIsInvoke() => isWaitUntilIsInvoke;
        public bool IsWaitUntilRateLimitDown() => isWaitUntilRateLimitDown;
    }
}
