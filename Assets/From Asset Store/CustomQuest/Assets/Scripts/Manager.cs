using UnityEngine;

/// <summary>
/// A manager class, used to make a monobehavior into a singleton, so everyone can acces it from anywhere.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Manager<T> : MonoBehaviour where T : Manager<T>
{
    private static bool isQuitting = false;

    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
            }
            if (instance == null && isQuitting != true)
            {
                Debug.LogError("Cannot find object of type: " + typeof(T).ToString() + "Cannot instanciate. Make sure one exists in the scene");
            }
            return instance;
        }

    }

    public virtual void Awake()
    {
        instance = this as T;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}