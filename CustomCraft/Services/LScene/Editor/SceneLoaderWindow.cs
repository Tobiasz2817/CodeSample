#if UNITY_EDITOR

using Object = UnityEngine.Object;
using UnityEditor.SceneManagement;
using CustomInspector;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace LScene.Editor {
    public class SceneLoaderWindow : EditorWindow
    {
        const int ButtonsPerLine = 4;

        EditorLoaderData _loaderData;
        int _selectedInitSceneIndex = 0;
        
        bool _isDragging = false;
        string _draggedScenePath = "";
        
        Vector2 _scrollPos;
        Vector2 _dragStartPosition;

        [MenuItem("Window/General/Open Scene Loader")]
        static void OpenWindow() => GetWindow(typeof(SceneLoaderWindow));

        void OnGUI() {
            DrawSequenceLoader();
            DrawOpenSceneByButton();
        }

        void OnEnable() {
            _loaderData = EditorLoaderData.LoadData();
            
            if (!string.IsNullOrEmpty(_loaderData.InitScenePath)) {
                var scenes = EditorBuildSettings.scenes;
                for (var i = 0; i < scenes.Length; i++) {
                    string sceneName =  scenes[i].path;

                    if (sceneName.Equals(_loaderData.InitScenePath)) {
                        _selectedInitSceneIndex = i;
                        break;
                    }
                }
            }
            else if (EditorBuildSettings.scenes.Length > 0)
                _loaderData.InitScenePath =  EditorBuildSettings.scenes[0].path;
        }
        
        void DrawSequenceLoader() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Space(3f);
                
                EditorGUILayout.BeginHorizontal();
                {
                    float fullWidth = EditorGUIUtility.currentViewWidth;
                    float innerWidth = fullWidth - (EditorStyles.helpBox.padding.horizontal * 2);

                    float isOnWidth = innerWidth * 0.1f;
                    float networkLoadingWidth = innerWidth * 0.1f;
                    float automaticConnection = innerWidth * 0.15f;
                    
                    GUI.color = _loaderData.CustomLoading ? Color.green : Color.red;
                    
                    string iconView = _loaderData.CustomLoading ? "✔" : "❌";
                    if (GUILayout.Button(iconView, GUILayout.Height(32), GUILayout.Width(isOnWidth))) {
                        _loaderData.CustomLoading = !_loaderData.CustomLoading;
                        EditorLoaderData.SaveData(_loaderData);
                    }
                    
                    GUI.color = Color.white;
                    
                    
                    GUI.color = _loaderData.NetworkLoading ? Color.green : Color.red;
                    
                    string networkLoadingIcon = _loaderData.NetworkLoading ? "🌐" : "❌";
                    if (GUILayout.Button(networkLoadingIcon, GUILayout.Height(32), GUILayout.Width(networkLoadingWidth))) {
                        _loaderData.NetworkLoading = !_loaderData.NetworkLoading;
                        EditorLoaderData.SaveData(_loaderData);
                    }
                    
                    GUI.color = Color.white;
                    
                    GUI.color = _loaderData.AutomaticConnection ? Color.green : Color.red;
                    
                    string automaticConnectionIcon = _loaderData.AutomaticConnection ? "🚀🌐" : "❌";
                    if (GUILayout.Button(automaticConnectionIcon, GUILayout.Height(32), GUILayout.Width(automaticConnection))) {
                        _loaderData.AutomaticConnection = !_loaderData.AutomaticConnection;
                        EditorLoaderData.SaveData(_loaderData);
                    }
                    
                    GUI.color = Color.white;
                    
                    string[] scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
                    string[] withoutPath = scenes.Select(Path.GetFileNameWithoutExtension).ToArray();
                    int index = EditorGUILayout.Popup(_selectedInitSceneIndex, withoutPath, GUI.skin.button, GUILayout.Height(32));
                    if (index != _selectedInitSceneIndex) {
                        _loaderData.InitScenePath = scenes[index];
                        EditorLoaderData.SaveData(_loaderData);
                        
                        _selectedInitSceneIndex =  index;
                    }
                } EditorGUILayout.EndHorizontal();
                
                
            } EditorGUILayout.EndVertical();
        }

        void DrawOpenSceneByButton()
       {
           var scenes = EditorBuildSettings.scenes;
           
           EditorGUILayout.BeginVertical(EditorStyles.helpBox);
           {
               EditorGUILayout.BeginHorizontal();
               {
                   GUILayout.Space(2f);
                   EditorGUILayout.LabelField("Scenes To Open", EditorStyles.boldLabel);
               } EditorGUILayout.EndHorizontal();
               
               GUILayout.Space(3f);
               
               _scrollPos = GUILayout.BeginScrollView(_scrollPos, true, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
               EditorGUILayout.BeginVertical();
               {
                   scenes.MakeEditorGrid((scene, _) =>
                   {
                       SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                       Rect buttonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 1.5f);

                       int controlID = GUIUtility.GetControlID(FocusType.Passive, buttonRect);
                       Event currentEvent = Event.current;
                       
                       switch (currentEvent.GetTypeForControl(controlID))
                       {
                           case EventType.MouseDown:
                           {
                               if (buttonRect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
                               {
                                   _isDragging = true;
                                   _dragStartPosition = currentEvent.mousePosition;
                                   _draggedScenePath = scene.path;
                                   GUIUtility.hotControl = controlID;
                                   currentEvent.Use();
                               }
                               
                           }
                               break;
                           case EventType.MouseDrag:
                               if (GUIUtility.hotControl == controlID && _isDragging && currentEvent.button == 0)
                               {
                                   if (Vector2.Distance(_dragStartPosition, currentEvent.mousePosition) > 5f)
                                   {
                                       DragAndDrop.PrepareStartDrag();
                                       DragAndDrop.objectReferences = new Object[] { sceneAsset };
                                       DragAndDrop.paths = new[] { _draggedScenePath };

                                       DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                                       DragAndDrop.StartDrag(System.IO.Path.GetFileNameWithoutExtension(_draggedScenePath));

                                       _isDragging = false;
                                       GUIUtility.hotControl = 0;
                                       currentEvent.Use();

                                       Resources.UnloadUnusedAssets();
                                   }
                               }
                               break;

                           case EventType.MouseUp:
                           { 
                               if (GUIUtility.hotControl == controlID || _isDragging)
                               {
                                   GUIUtility.hotControl = 0;

                                   if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                       EditorSceneManager.OpenScene(_draggedScenePath);
                                   
                                   _isDragging = false;
                                   _draggedScenePath = "";
                                   currentEvent.Use();
                               }
                           }
                               break;

                           case EventType.Repaint:
                               GUI.Button(buttonRect, System.IO.Path.GetFileNameWithoutExtension(scene.path));
                               break;
                       }
                   }, ButtonsPerLine);
               }
               
               EditorGUILayout.EndVertical();
               GUILayout.EndScrollView();
           } EditorGUILayout.EndVertical();
       }
    }
}


#endif