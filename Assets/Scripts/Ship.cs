using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Ship : MonoBehaviour {

    public float pitchRate = 0.3f;
    public float yawRate = 0.2f;
    public float thrustRate = 1f;

    public float stopSeconds = 0.5f;
    public float maxEngineVolume = 0.8f;

    public float maxSpeed = 1500f;

    public ParticleSystem spaceDust;
    public AudioSource engineSound;
    public AudioSource dustSound;
    public GameObject[] weapons;
    public GameObject squad;

    public Rigidbody body;

    private AutoPilot _AI;

    public AutoPilot AI
    {
        get
        {
            return _AI;
        }
        set
        {
            string old_AI = null;

            if (_AI == null)
            {
                old_AI = "Nothing";
            } else
            {
                old_AI = _AI.GetType().Name;

                _AI.UnInstall(this, body);
            }

            if (value == null)
                Debug.LogWarningFormat("{0} AI set to Nothing", this.PathID());

            _AI = value;

            if (_AI == null)
            {
                Debug.LogFormat("{0} change state from {1} to null", this.PathID(), old_AI);
            } else {
                Debug.LogFormat("{0} change state from {1} to {2}", this.PathID(), old_AI, _AI.GetType().Name);
            }

            _AI.Install(this, body);
        }
    }

    public AutoPilot[] pilots;

    private GameObject _target;
    public GameObject target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;

            if (AI == null) return;

            AI.OnTargetChanged(this, body);
        }
    }

    // Use this for initialization
    void Awake () {
        body = GetComponentInChildren<Rigidbody>();
        if(body == null)
        {
            Debug.LogWarningFormat("ship cannot find a Rigidbody in it's children");
        }

        pilots = new AutoPilot[(int)AutoPilot.pilots.LAST];

        pilots[(int)AutoPilot.pilots.None] = new AutoPilot();
        pilots[(int)AutoPilot.pilots.Player] = new PlayerPilot();
        pilots[(int)AutoPilot.pilots.Stop] = new PilotStop();
        pilots[(int)AutoPilot.pilots.AI] = new PlayerPilot(); // new AIPilot();
        pilots[(int)AutoPilot.pilots.Intercept] = new PilotInterceptor();

        AI = pilots[(int)AutoPilot.pilots.Player];
    }
	
	// Update is called once per frame
	void Update () {
        if (dustSound != null)
        {
            float dustVolume = Mathf.Clamp(body.velocity.magnitude, 0f, maxSpeed) / maxSpeed;

            //Debug.LogFormat("velocity {0}, dust volume {1}", body.velocity.magnitude, dustVolume);

            dustSound.volume = dustVolume;
        }

        AI.Pilot(this, body);
    } // end Update
}
