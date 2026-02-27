using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour

    where T : MonoBehaviour {
    private static T _instance;
    public static T Instance {
        get {
            if(_instance == null) {
                _instance = FindFirstObjectByType<T>();

                if(_instance == null) {
                    Debug.LogError(
                        $"Singleton of type {typeof(T)} not found in scene."
                    );
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake() {
        if(_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
    }
}