using UnityEngine;
using System.Collections;

public class PlayerPilot : AutoPilot
{
    public float deadZone = 0.045f;

    public override void Install(Ship ship, Rigidbody body)
    {

    }

    public override void UnInstall(Ship ship, Rigidbody body)
    {

    }

    public override void Pilot(Ship ship, Rigidbody body)
    {
        if (RadialMenu.IsOpen) return;

        if (Input.GetButton("Stop"))
        {
            ship.pilots[(int)AutoPilot.pilots.Stop].Pilot(ship, body);
        }
        else
        {
            float minEngineVolume = 0f;
            float pitch = Input.GetAxis("Mouse X");
            float yaw = -1.0f * Input.GetAxis("Mouse Y");

            if (Mathf.Abs(pitch) < deadZone) pitch = 0f;
            if (Mathf.Abs(yaw) < deadZone) yaw = 0f;

            //pitch = pitch * 10f;
            //pitch = (pitch * Mathf.Abs(pitch)) / 10f;

            //yaw = yaw * 10f;
            //yaw = (yaw * Mathf.Abs(yaw)) / 10f;

            if (pitch != 0f || yaw != 0f)
            {
                pitch = Mathf.Clamp(pitch, -1f, 1f);
                yaw = Mathf.Clamp(yaw, -1f, 1f);

                minEngineVolume = (Mathf.Abs(pitch) + Mathf.Abs(yaw)) / 2f;
                // Debug.LogFormat("Mouse ({0},{1})", pitch, yaw);
                //if (pitch < 0f) pitch = -1f;
                //if (pitch > 0f) pitch = 1f;

                //if (yaw < 0f) yaw = -1f;
                //if (yaw > 0f) yaw = 1f;

                Vector3 vPitch = body.transform.up * ship.pitchRate * Time.deltaTime * pitch;
                Vector3 vYaw = body.transform.right * ship.yawRate * Time.deltaTime * yaw;

                body.angularVelocity += vPitch + vYaw;
            }

            float thrust = Input.GetAxis("Thrust");

           // Debug.LogFormat("PlayerPilot Thrust at {0}", thrust);

            if (thrust == 0f)
            {
                ship.engineSound.volume = minEngineVolume;
            }
            else
            {
                Vector3 vThrustForce = body.transform.forward * ship.thrustRate * thrust * Time.deltaTime;
                body.AddForce(vThrustForce, ForceMode.Impulse);
                ship.engineSound.volume = minEngineVolume + ((ship.maxEngineVolume - minEngineVolume) * thrust);
            }

            ////spaceDust.transform.rotation = Quaternion.Euler(-1f * body.velocity);
            //var pmm = spaceDust.main;
            //pmm.startSpeed = body.velocity.magnitude * 0.3f;
            //var pme = spaceDust.emission;
            //pme.rateOverTime = body.velocity.magnitude;
        }
    }
}
