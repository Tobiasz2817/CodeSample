using UnityEngine;
using System;

namespace Storex {
    public class StorexExample : MonoBehaviour {
        //[Header("Base Data")]
        public BaseData BaseDataHolder;
        public Position position;

        void Update() {
            // Load - Save Base Data
            if (Input.GetKeyDown(KeyCode.Q)) {
                var data = new BaseData {
                    Damage = 5,
                    Health = 10,
                    Name = "XD",
                };
                
                StorexService.Save(data);
            }
            if (Input.GetKeyDown(KeyCode.W)) {
                BaseDataHolder = StorexService.Load<BaseData>();
            }
            
            
            // Load - Position
            if (Input.GetKeyDown(KeyCode.E)) {
                var data = new Position {
                    X = 2,
                    Y = 2,
                    Z = 2,
                };
                
                StorexService.Save(data);
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                position = StorexService.Load<Position>();
            }
        }


        void OnGUI() {
            if (GUILayout.Button("XD")) {
                
            }
        }

        [Serializable]
        public struct BaseData {
            public float Damage;
            public float Health;
            public string Name;
        }
        
        [Serializable]
        public struct Id {
            public float[] Ids;
        }
        
        [Serializable]
        public class Position {
            public float X = 5;
            public float Y = 2;
            public float Z = 1;
        }
    }
}