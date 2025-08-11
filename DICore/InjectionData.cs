using System;
using System.Collections.Generic;
using System.Reflection;

namespace DICore {
    public class InjectionData<TInject, TProvide> {
        // Searching in base class with instances, public and private members
        internal BindingFlags InjectFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        // Searching in base class with instances, public members. This mean only public parameter will read (no k_backflag)
        internal BindingFlags ProvideFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
        
        // Reading only fields and properties as default
        internal MemberTypes InjectMemberTypes = MemberTypes.Field | MemberTypes.Property;
        internal MemberTypes ProvideMemberTypes = MemberTypes.Field | MemberTypes.Property;

        // Checking assigned values
        internal bool InjectMemberAssignedCheck = true;
        internal bool ProvideMemberAssignedCheck = false;

        // Allows providers treat them like an value, not to read for reflection
        internal bool ProvidersAsValue = false;
        
        // Condition
        internal Func<(TInject, MemberInfo), (TProvide, MemberInfo), bool> Predicate;
        
        // Injectors/Providers
        internal IEnumerable<TInject> Injections;
        internal IEnumerable<TProvide> Providers;

        // Filter Attributes 
        internal Type InjectAttribute;
        internal Type ProvideAttribute;
    }
}