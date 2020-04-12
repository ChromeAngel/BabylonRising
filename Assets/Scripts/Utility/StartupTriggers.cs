using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ExtensionMethods;

public class StartupTriggers : MonoBehaviour {

    public UnityEngine.Events.UnityEvent OnAwake;
    public UnityEngine.Events.UnityEvent OnStart;
    public UnityEngine.Events.UnityEvent OnFirstFrame;

    private void Awake()
    {
        if (OnAwake != null)
        {
            Debug.LogFormat("{0} started", this.PathID());
            OnAwake.Invoke();
        }
    }
    private void Start()
    {
        if(OnStart != null)
        {
            Debug.LogFormat("{0} started", this.PathID());
            OnStart.Invoke();
        }
    }

    private void Update()
    {
        if (OnFirstFrame != null)
        {
            Debug.LogFormat("{0} started", this.PathID());
            OnFirstFrame.Invoke();
        }

        this.enabled = false;
    }
}
