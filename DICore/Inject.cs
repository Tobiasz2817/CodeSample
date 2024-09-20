using System;

namespace DICore {
        
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Inject : Attribute { }
}