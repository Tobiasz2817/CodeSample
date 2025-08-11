using UnityEngine.Scripting;
using Ability;
using UnityEngine;
using Inflowis;
using LScene;

#if UNITY_EDITOR
using LScene.Editor;
#endif

namespace Utilities
{
    public class Boot : MonoBehaviour
    {
        void Start()
        {
#if UNITY_EDITOR
            EditorLoadingSequence();
#else
            GameLoadingSequence();
#endif
        }
        
        [Preserve]
        void GameLoadingSequence()
        {
            // Consoles contains only local mode
            if (Application.isConsolePlatform)
            {
                InflowisLocal.Initialize();
                
                _= SceneLoader.ChangeScene(MainScene.LocalMenu);
            }
            else
            {
                InflowisCore.Initialize();
                KeyHintSystem.Initialize();
            }
        }
        
#if UNITY_EDITOR
        void EditorLoadingSequence()
        {
            InflowisLocal.Initialize();
            LobbyData.PlayersData = new PlayerData?[InflowisLocal.InputManager.maxPlayerCount];
            
            ESceneLoader.RunLoadingSequence();
        }
#endif
        
    }
}
