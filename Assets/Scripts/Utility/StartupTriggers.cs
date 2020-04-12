using UnityEngine;
using UnityEngine.Events;
using ExtensionMethods;
using UnityEngine.SceneManagement;

/// <summary>
/// GameObject lifetime events, without more scripting
/// </summary>
public class StartupTriggers : MonoBehaviour {

    public static bool Logging = false;

    public UnityEvent OnAwake = new UnityEvent();
    public UnityEvent OnStart = new UnityEvent();
    public UnityEvent OnFirstFrame = new UnityEvent();
    public UnityEvent OnUnload = new UnityEvent();
    public UnityEvent OnQuit = new UnityEvent();

    private bool disableAfterFirstFrame;

    private void Awake()
    {
        if(Logging)
            Debug.LogFormat("{0} started", this.PathID());

        OnAwake.Invoke();

        if(OnUnload.GetPersistentEventCount() > 0)
        {
            SceneManager.sceneUnloaded += OnSceneUnload;
        }
    }
    private void Start()
    {
        if (Logging)
            Debug.LogFormat("{0} started", this.PathID());

        OnStart.Invoke();
    }

    private void Update()
    {
        if (Time.frameCount == 1)
        {
            if (Logging)
                Debug.LogFormat("{0} FirstFrame", this.PathID());

            OnFirstFrame.Invoke();
            OnFirstFrame.RemoveAllListeners();
            OnFirstFrame = null;

            if( (OnUnload.GetPersistentEventCount() == 0 && OnQuit.GetPersistentEventCount() == 0))
            {
                if (Logging)
                    Debug.LogFormat("{0} disabled itself", this.PathID());

                this.enabled = false; //we're not expected to fire any more events, so we don't need to be updating every frame
            }
        }
    }

    private void OnSceneUnload(Scene scene)
    {
        if (Logging)
            Debug.LogFormat("{0} Unloaded", this.PathID());

        SceneManager.sceneUnloaded -= OnSceneUnload;
        OnUnload.Invoke();
    }

    private void OnApplicationQuit()
    {
        if (Logging)
            Debug.LogFormat("{0} Quit", this.PathID());

        OnQuit.Invoke();
    }
}
