using UnityEngine;
using UnityEngine.Events;
using ExtensionMethods;

/// <summary>
/// Singleton Utility to Pause and Resume the flow of time, check for bound key presses and Quit the game
/// </summary>
public class PauseAndQuit : MonoBehaviour {
    //singleton instance accessor
    public static PauseAndQuit Instance;

    /// <summary>
    /// All the keys that are bounds to commands
    /// </summary>
    public KeyBinds keyBindings;

    /// <summary>
    /// The Quit command that may be bound to a key
    /// </summary>
    public Command quitCommand;

    /// <summary>
    /// the Pause command  that may be bound to a key
    /// </summary>
    public Command pauseCommand;

    /// <summary>
    /// An event that happens when the game is Paused
    /// </summary>
    public UnityEvent OnPause = new UnityEvent();

    /// <summary>
    /// An event that happens when the game Resumes after being paused
    /// </summary>
    public UnityEvent OnResume = new UnityEvent();

    /// <summary>
    /// Should time be paused when the scene starts?
    /// </summary>
    public bool StartPaused = false;

	/// <summary>
    /// Initialization, litsen for commands, pausing on start
    /// </summary>
	void Start () {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogWarningFormat("Multiple instances of MenuHotKeys found in scene {0} and {1}, disabling {1}",
                    Instance.PathID(),
                    this.PathID());

            enabled = false;
        }

        if(quitCommand)
        {
            quitCommand.OnRelease.AddListener(Quit);
        }
        
        if(pauseCommand)
        {
            pauseCommand.OnRelease.AddListener(TogglePause);

            if (StartPaused)
            {
                Pause();
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
	
	/// <summary>
    /// Check for bound key presses each frame
    /// </summary>
	void Update () {
        if(keyBindings)
        {
            keyBindings.CheckForCommands();
        }
    }

    /// <summary>
    /// Toggle the pause state of the game
    /// </summary>
    /// <param name="ignore">unused parameter for how long the quit key was held</param>
    /// <remarks>private instance bound to the Pause command release event</remarks>
    private void TogglePause(float ignore)
    {
        TogglePause();
    }

    /// <summary>
    /// Toggle the pause state of the game
    /// </summary>
    public void TogglePause()
    {
        if (IsPaused())
        {
            Resume();   
        }
        else
        {
            Pause();   
        }
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    /// <param name="ignore">unused parameter for how long the quit key was held</param>
    /// <remarks>private instance bound to the Quit command release event</remarks>
    private void Quit(float ignore)
    {
        Quit();
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    private void Quit()
    {
        Pause();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
# endif
    }

    /// <summary>
    /// Pause the game when it looses focus
    /// </summary>
    /// <param name="focus">does the game have focus</param>
    private void OnApplicationFocus(bool focus)
    {
        //Pause on lost focus
        if (!focus && !IsPaused())
            Pause();
    }

    /// <summary>
    /// Is the game paused?
    /// </summary>
    /// <returns>Is the game paused?</returns>
    public bool IsPaused()
    {
        return (Time.timeScale == 0f);
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Paused");

        OnPause.Invoke();

        
    }

    /// <summary>
    /// Resume the game from being paused
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("Playing");

        OnResume.Invoke();

        
    }

}
