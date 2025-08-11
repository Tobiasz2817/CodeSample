using System;

namespace ModuleSystem {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Inject : Attribute { }
}