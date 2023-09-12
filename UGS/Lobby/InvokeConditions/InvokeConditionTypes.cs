namespace GameZone.Multiplayer.UGS.Lobby.InvokeCondition
{
    public class InvokeConditionTypes
    {
        public static InvokeCondition OnlyIsInvoke = new InvokeCondition(true,false,false);
        public static InvokeCondition OnlyWaitUntilRateLimit = new InvokeCondition(false,false,true);
        public static InvokeCondition OnlyInvokeAndWaitInvoke = new InvokeCondition(true,true,false);
        public static InvokeCondition AllTrue = new InvokeCondition(true,true,true);
        public static InvokeCondition AllFalse = new InvokeCondition(false,false,false);
    }
}