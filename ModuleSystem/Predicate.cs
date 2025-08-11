using System;

namespace ModuleSystem {
    public class Predicate : ICondition {
        private Func<bool> _condition;
        public Predicate(Func<bool> condition) {
            _condition = condition;
        }
        public bool Evaluate() => _condition != null && _condition.Invoke();
    }
}