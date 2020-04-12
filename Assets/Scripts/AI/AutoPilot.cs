using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPilot {

    public enum pilots
    {
        None,
        Player,
        Stop,
        AI,
        Intercept,
        LAST
    }

    public virtual void Install(Ship ship, Rigidbody body)
    {

    }

    public virtual void UnInstall(Ship ship, Rigidbody body)
    {

    }

    public virtual void Pilot (Ship ship, Rigidbody body) {
		
	}

    public virtual void OnTargetChanged(Ship ship, Rigidbody body) {

    }
}
