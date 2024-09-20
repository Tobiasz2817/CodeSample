using UnityEditor;
using UnityEngine;

namespace CoreUtility
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    public class SingletonPersistent<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    [InitializeOnLoad]
    public class Persistent<T> : MonoBehaviour where T : Component
    {
        protected static T Instance { get; private set; }
        static Persistent() {
            GameObject newGameObject = new GameObject(typeof(T).ToString());
            Instance = newGameObject.AddComponent<T>();
            DontDestroyOnLoad(Instance);
        }
    }
}
