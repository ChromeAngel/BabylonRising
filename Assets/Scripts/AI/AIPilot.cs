using UnityEngine;
using System.Collections;

public class AIPilot : AutoPilot
{
    public override void Pilot(Ship ship, Rigidbody body)
    {
        //TODO decide which AutoPilot to switch to
    }

    public override void OnTargetChanged(Ship ship, Rigidbody body)
    {

    }
}
