using CoreUtility.Extensions;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using CoreUtility;

namespace Ability {
    public class AbilityConfig : ScriptableObject {
        [SerializeField]
        [BoxGroup("General", ShowLabel = false)]
        [TitleGroup("General/Settings")]
        internal bool LoadInMemory;
        
        [SerializeField]
        [BoxGroup("ConfigGroup", ShowLabel = false)]
        [TitleGroup("ConfigGroup/Config")]
        internal ConfigData FileConfig;
        
        [Button]
        [TitleGroup("ConfigGroup/Config")]
        [ButtonGroup("ConfigGroup/Config/Buttons")]
        void ChangePath() => AbilitiesCore.MoveConfig(ref FileConfig);
        
        [Button]
        [TitleGroup("ConfigGroup/Config")]
        [ButtonGroup("ConfigGroup/Config/Buttons")]
        void ChangeName() => AbilitiesCore.ChangeConfigName(ref FileConfig);
        
        [SerializeField]
        [BoxGroup("ImporterGroup", ShowLabel = false)]
        [TitleGroup("ImporterGroup/Importer")]
        internal string[] PathList;
        [SerializeField]
        [TitleGroup("ImporterGroup/Importer")]
        internal SearchType SearchType;

        [SerializeField]
        [TitleGroup("FindGroup/Finder")]
        [BoxGroup("FindGroup", ShowLabel = false)]
        internal string FolderName;
        [ReadOnly]
        [SerializeField]
        [TitleGroup("FindGroup/Finder")]
        internal string CoreFolderName = "Assets";

        [Button]
        [TitleGroup("FindGroup/Finder")]
        [ButtonGroup("FindGroup/Finder/Buttons")]
        void SearchFolder() {
            if (string.IsNullOrEmpty(FolderName)) 
                return;
            
            var path = FileHelper.FindFolder(CoreFolderName, FolderName);
            if (string.IsNullOrEmpty(path)) {
                Debug.LogError("Folder Name didn't contains in repository");
                return;
            }
            
            FolderName = string.Empty;

            if (PathList.Contains(path)) {
                Debug.LogWarning("Folder exist in importer array");
                return;
            }
            
            PathList = PathList.Add(path);
        }
    }
}