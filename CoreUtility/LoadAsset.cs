using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CoreUtility.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CoreUtility {
    public static class AddressableLoad {
        /// <param name="searchType"> Inside: research the asset in side the class, value is the class </param>
        /// <param name="paths"> The list of wanted to research path</param>
        /// <returns> List of the wanted references </returns>
        [Tooltip("Importing all data from the entry path")]
        public static async Task<List<T>> Import<T>(SearchType searchType = SearchType.Inside, params string[] paths) {
            var list = new List<T>();
            var tasks = new List<Task>();
            
            switch (searchType) {
                case SearchType.Value: {
                    foreach (var path in paths) {
                        var handle = Addressables.LoadAssetsAsync<T>(path, (obj => {
                            list.Add(obj);
                        }));

                        tasks.Add(handle.Task);
                    }
                    
                    break;
                }
                case SearchType.Inside: {
                    var insideFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
                    var membersFlag = MemberTypes.Field | MemberTypes.Property;

                    foreach (var path in paths) {
                        var handle = Addressables.LoadAssetsAsync<Object>(path, (obj) => {
                            var providerMembers = obj.GetMembers(membersFlag, insideFlags);
                
                            foreach (var member in providerMembers) {
                                var value = member.GetMemberValue(obj);
                                if(value is not T target) continue;
                                list.Add(target);
                            }
                        });

                        tasks.Add(handle.Task);
                    }
                    break;
                }
            }
            
            await Task.WhenAll(tasks);
            return list;
        }
    }

    public static class ResourcesLoad {
        
    }


    public static class EditorLoad {
        /// <param name="searchType"> Inside: research the asset in side the class, value is the class </param>
        /// <param name="paths"> The list of wanted to research path</param>
        /// <returns> List of the wanted references </returns>
        [Tooltip("Importing all data from the entry path of Editor")]
        public static List<T> Import<T>(SearchType searchType = SearchType.Inside, params string[] paths) {
            List<T> list = new List<T>();
            var guids = AssetDatabase.FindAssets(string.Empty, paths);
            
            switch (searchType) {
                case SearchType.Value: {
                    //TODO::
                    break;
                }
                case SearchType.Inside: {
                    var insideFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
                    var membersFlag = MemberTypes.Field | MemberTypes.Property;
                    
                    foreach (var guid in guids) {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject));
                        if(asset == null) continue;
                        
                        var providerMembers = asset.GetMembers(membersFlag, insideFlags);
                        
                        foreach (var member in providerMembers) {
                            var value = member.GetMemberValue(asset);
                            if(value is not T target) continue;
                            list.Add(target);
                        }
                    }

                    break;
                }
            }
            
            return list;
        }
    }
    
    public enum SearchType {
        Inside,
        Value
    }
}