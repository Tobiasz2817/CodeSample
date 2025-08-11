using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using UnityEngine;

namespace Inflowis
{
    public static class InflowisLocal
    {
        public static PlayerInputManager InputManager;
        
        public static void Initialize()
        {
            Assert.IsTrue(InputManager == null, "Input manager is initialized!");
            
            Application.quitting -= Reset;
            Application.quitting += Reset;
            
            var config = Resources.Load<InflowisConfig>(InflowisCore.ConfigName);
            
            InputManager = Object.Instantiate(config.InputManagerPrefab);
            
            var inputManager = InputManager;
            inputManager.name = inputManager.name.Replace("(Clone)", "");
            Object.DontDestroyOnLoad(inputManager);
            
            InputManager.onPlayerJoined += input =>
            {
                input.transform.SetParent(InputManager.transform);
            };
        }

        static void Reset() => InputManager = null;
    }
}