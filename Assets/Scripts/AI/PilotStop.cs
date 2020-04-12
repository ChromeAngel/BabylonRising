using UnityEngine;
using System.Collections;

public class PilotStop : AutoPilot
{
    public float stopStart;
    public Vector3 stoppingSpeed;
    public Vector3 stoppingSpin;

    public override void Install(Ship ship, Rigidbody body)
    {

    }

    public override void UnInstall(Ship ship, Rigidbody body)
    {

    }

    public override void Pilot(Ship ship, Rigidbody body)
    {
        if (Input.GetButtonDown("Stop"))
        {
            stopStart = Time.time;
            stoppingSpeed = body.velocity;
            stoppingSpin = body.angularVelocity;
        }

        float stopRatio = Mathf.Min(Time.time - stopStart, ship.stopSeconds) / ship.stopSeconds;

        //Debug.LogFormat("stoping {0}", stopRatio);

        body.angularVelocity = Vector3.Lerp(stoppingSpin, Vector3.zero, stopRatio);
        body.velocity = Vector3.Lerp(stoppingSpeed, Vector3.zero, stopRatio);

        if (body.velocity.magnitude > 0.01f)
        {
            ship.engineSound.volume = ship.maxEngineVolume;
        }
    }
}
