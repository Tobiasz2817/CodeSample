using System.Collections.Generic;
using CoreUtility;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEngine.AddressableAssets;
#endif

namespace Inflowis {
    public static class InputDatabase {
        #region Icons
        
        public static async Awaitable<IList<Sprite>> LoadIconsAsync(DeviceName deviceType) {
            try {
                string targetLabel = "Binds-" + deviceType;
                
#if UNITY_EDITOR
                var locations = Addressables.LoadResourceLocationsAsync(targetLabel).WaitForCompletion();

                if (!locations.Any()) {
                    Debug.LogWarning("No icons found for: " + deviceType);
                    return new List<Sprite>();
                }
#endif
                
                return await AddressableLoad.LoadAssets<Sprite>(targetLabel);
            }
            catch (Exception _) {
                return new List<Sprite>();
            }
        }

        #endregion
    }
}