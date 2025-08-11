using System.Collections.Generic;
using CoreUtility.Extensions;
using System.Reflection;
using System.Linq;
using System;

namespace DICore {
    public static class DI {
        public static InjectionData<TInject, TProvide> CreateData<TInject, TProvide>() {
            return new InjectionData<TInject, TProvide>();
        }
        public static InjectionData<TInject, TProvide> WithITypes<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, MemberTypes memberTypes) {
            injectionData.InjectMemberTypes = memberTypes;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithPTypes<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, MemberTypes memberTypes) {
            injectionData.ProvideMemberTypes = memberTypes;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithCondition<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, Func<(TInject, MemberInfo), (TProvide, MemberInfo), bool> predicate) {
            injectionData.Predicate = predicate;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithIAttribute<TInject, TProvide> (this InjectionData<TInject, TProvide> injectionData, Type attribute) {
            injectionData.InjectAttribute = attribute;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithIFlag<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, BindingFlags flag) {
            injectionData.InjectFlag = flag;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithPFlag<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, BindingFlags flag) {
            injectionData.ProvideFlag = flag;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithInjections<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, IEnumerable<TInject> injects) {
            injectionData.Injections = injects;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithProviders<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, IEnumerable<TProvide> provides) {
            injectionData.Providers = provides;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithAssignedCheck<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, bool injectMemberAssignedCheck = true, bool provideMemberAssignedCheck = false) {
            injectionData.InjectMemberAssignedCheck = injectMemberAssignedCheck;
            injectionData.ProvideMemberAssignedCheck = provideMemberAssignedCheck;
            return injectionData;
        }
        
        public static InjectionData<TInject, TProvide> WithPAsValue<TInject, TProvide>(this InjectionData<TInject, TProvide> injectionData, bool providersAsValue = true) {
            injectionData.ProvidersAsValue = providersAsValue;
            return injectionData;
        }

     
        
        public static void Inject<TInject, TProvide>(this InjectionData<TInject, TProvide> data) {
            foreach (var injection in data.Injections) {
                var injectionMembers = injection.GetMembers(data.InjectMemberTypes, data.InjectFlag).Where((member) =>
                    (data.InjectAttribute == null || Attribute.IsDefined(member, data.InjectAttribute)) && 
                    (!data.InjectMemberAssignedCheck || member.GetMemberValue(injection).IsAssigned()));
                
                foreach (var injectionMember in injectionMembers) {
                    foreach (var provider in data.Providers) {
                        var providerMembers = !data.ProvidersAsValue ? provider.GetMembers(data.ProvideMemberTypes, data.ProvideFlag).Where((member) =>
                            (data.ProvideAttribute == null || Attribute.IsDefined(member, data.ProvideAttribute)) &&
                            (!data.ProvideMemberAssignedCheck || member.GetMemberValue(provider).IsAssigned())) : null;

                        if (providerMembers != null) {
                            foreach (var providerMember in providerMembers) 
                                if (TryReferValue(injectionMember, providerMember, injection, provider))
                                    break;
                            
                            break;
                        }

                        TryReferValue(injectionMember, null, injection, provider);
                    }
                }
            }

            return;
            bool TryReferValue(MemberInfo injectionMember, MemberInfo providerMember, TInject injection, TProvide provider) {
                if (!injectionMember.IsMemberType(providerMember != null ? providerMember.GetMemberType() : provider.GetType()) || 
                    (data.Predicate != null && !data.Predicate.Invoke((injection, injectionMember), (provider, providerMember))))
                    return false;
                        
                injectionMember.SetMemberValue(injection, providerMember == null ? provider : providerMember.GetMemberValue(provider));
                return true;
            }
        }
    }
}