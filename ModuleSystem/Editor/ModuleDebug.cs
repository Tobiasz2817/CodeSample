using UnityEditor;
using UnityEngine;

namespace ModuleSystem.Editor {
    [CustomEditor(typeof(ModuleController))]
    public class ModuleDebug : UnityEditor.Editor {
        public override void OnInspectorGUI() { 
            base.OnInspectorGUI();
            var controller = (ModuleController)target;
            if (!controller.displayDebug) return;
            GUILayout.BeginHorizontal();

            var debugTextStyle = new GUIStyle(EditorStyles.label){
               normal = { textColor = Color.red },
               fontSize = 14,
               fontStyle = FontStyle.Bold,
               alignment = TextAnchor.MiddleCenter
            };

            var infoTextStyle = new GUIStyle(EditorStyles.label){
               normal = { textColor = Color.white },
               fontSize = 10,
               fontStyle = FontStyle.Bold,
               alignment = TextAnchor.MiddleCenter
            };

            GUILayout.Label("---Debug---", debugTextStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            // Info
            var cText = "Current";
            GUILayout.Label(cText, infoTextStyle);
            DrawColoredCircle(Color.green, 5, GUILayoutUtility.GetLastRect(), cText.Length * 4);

            var pText = "Previous";
            GUILayout.Label(pText, infoTextStyle);
            DrawColoredCircle(Color.yellow, 5, GUILayoutUtility.GetLastRect(), pText.Length * 4);

            var eText = "Expansion";
            GUILayout.Label(eText, infoTextStyle);
            DrawColoredCircle(Color.red, 5, GUILayoutUtility.GetLastRect(), eText.Length * 4f);

            var nuText = "Not Usage";
            GUILayout.Label(nuText, infoTextStyle);
            DrawColoredCircle(Color.white, 5, GUILayoutUtility.GetLastRect(), nuText.Length * 4);

            GUILayout.EndHorizontal();

            GUILayout.Space(30f);

            if (!Application.isPlaying) return;
            // Modules
            var moduleTextStyle = new GUIStyle(EditorStyles.label){
               fontSize = 13,
               fontStyle = FontStyle.Bold,
               alignment = TextAnchor.MiddleCenter
            };
            foreach (var module in controller.GetModules()) {
               if (controller.Current == module) 
                   moduleTextStyle.normal.textColor = Color.green;
               else if(controller.IsModuleUsageAsExtension(module))
                   moduleTextStyle.normal.textColor = Color.red;
               else if (controller.PreviousState == module) 
                   moduleTextStyle.normal.textColor = Color.yellow;
               else 
                   moduleTextStyle.normal.textColor = Color.white;
               
               GUILayout.Label(module?.GetType().Name, moduleTextStyle);
            }

            EditorUtility.SetDirty(target);
        }
        
        void DrawColoredCircle(Color color, float radius, Rect labelRect, float xAddons = 0, float yAddons = 0) {
            Vector2 center = new Vector2(labelRect.center.x + xAddons, labelRect.center.y + yAddons);
            
            Handles.BeginGUI();
            Color previousColor = Handles.color;
            Handles.color = color;
            Handles.DrawSolidDisc(center, Vector3.forward, radius);
            Handles.color = previousColor;
            Handles.EndGUI();
        }
    }
}