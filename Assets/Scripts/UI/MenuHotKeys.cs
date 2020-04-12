using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHotKeys : MonoBehaviour {
    public bool StartPaused = false;

	// Use this for initialization
	void Start () {
        if(StartPaused) Pause();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Cancel"))
        {
            if(IsPaused())
            {
                Resume();
            } else
            {
                Pause();
            }
        }

        if (Input.GetButton("Quit"))
        {
            QuitGame();
        }
    }

    public static bool IsPaused()
    {
        return (Time.timeScale == 0f);
    }

    public static void Pause()
    {
        Time.timeScale = 0f;
        Debug.Log("Paused");
    }

    public static void Resume()
    {
        Time.timeScale = 1f;
        Debug.Log("Playing");
    }

    public static void QuitGame()
    {
        Pause();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
