namespace ModuleSystem {
    public class Transition : ITransition {
        public int ToId { get; }
        public ICondition Condition { get; }
        
        public Transition(int to, ICondition condition) {
            ToId = to;
            Condition = condition;
        }
    }
}