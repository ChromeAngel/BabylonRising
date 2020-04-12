using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;

public class PilotInterceptor : AutoPilot
{
    private List<courseWaypoint> course = new List<courseWaypoint>();
    private courseWaypoint startWaypoint;
    private float pathRadius = 10f;
    private float waypointTollerance = 50f;

    public override void Install(Ship ship, Rigidbody body)
    {

    }

    public override void UnInstall(Ship ship, Rigidbody body)
    {

    }

    public override void Pilot(Ship ship, Rigidbody body)
    {
        if (Time.timeScale == 0f) return; // don't move when paused

        if(IsOnCollisionCourse(ship, body))
        {
            PlotCourseToTarget(ship, body);
        }

        if (course == null || course.Count == 0)
        {
            PlotCourseToTarget(ship, body);

            if (course == null || course.Count == 0)
            {
                Debug.Log("No course or no more waypoints");

                ship.AI = ship.pilots[(int)AutoPilot.pilots.AI];

                return;
            }
        }

        courseWaypoint waypoint = course[0];
        if(Vector3.Distance(ship.transform.position , waypoint.position) <= waypointTollerance )
        {
            //if we're close enough to our waypoint, let's start aiming for the next one
            course.Remove(waypoint);

            if (course.Count == 0)
            {
                Debug.Log("Arrived at last waypoint");

                ship.AI = ship.pilots[(int)AutoPilot.pilots.AI];

                return;
            }

            waypoint = course[0];
        }

        
        var targetVectors = ship.transform.PitchAndYaw(waypoint.position);
        float pitch = 0f;
        float yaw = 0f;

        yaw   = Mathf.Clamp(targetVectors.x, -1f * ship.yawRate  , ship.yawRate);
        pitch = Mathf.Clamp(targetVectors.y, -1f * ship.pitchRate, ship.pitchRate);

        Debug.LogFormat("InterceptorPilot tv=({0},{1}) yaw={2} pitch={3}", targetVectors.x, targetVectors.y, yaw, pitch);

        //This faces us at the waypoint, but does nothing to ensure we're looking at the direction the waypoint specifies

        Vector3 vPitch = ship.transform.up * Time.deltaTime * pitch;
        Vector3 vYaw = ship.transform.right * Time.deltaTime * yaw;

        body.angularVelocity += vPitch + vYaw;

        float thrust = 0f;
        float velocity = body.velocity.magnitude;
        float ETA = targetVectors.z / velocity;
        float OptimumArrivalTime = targetVectors.z / (ship.maxSpeed / 2f);
        float timeDiff = Mathf.Abs(ETA - OptimumArrivalTime);
        
        if( ETA > 1f && timeDiff < 0.5f)
        {
            //close enough 
        } else
        {
            Vector3 vThrustForce = ship.transform.forward * ship.thrustRate * thrust;

            if (ETA > OptimumArrivalTime)
            {
                //need to accelerate
            }
            else
            {
                //need to decelerate, reverse thrust
                vThrustForce = vThrustForce * -1f;
            }

            body.AddForce(vThrustForce, ForceMode.Force);
        }

    }

    private bool IsOnCollisionCourse(Ship ship, Rigidbody body)
    {
        var x = Oracle.Instance.PredictCollison(ship.GetComponent<Predictable>(), 1f);
        return (x.other != null);
    }

    public override void OnTargetChanged(Ship ship, Rigidbody body)
    {
        PlotCourseToTarget(ship, body);
    }

    protected virtual void PlotCourseToTarget(Ship ship, Rigidbody body)
    {
        course.Clear();

        startWaypoint = new courseWaypoint()
        {
            position = ship.transform.position,
            rotation = ship.transform.rotation,
            velocity = body.velocity
        };

        if (ship.target == null)
        {
            course = null;
            return;
        }

        PlotCourseTo(ship.transform.position, ship.target.transform.position, ship);
    }

    protected void PlotCourseTo(Vector3 start, Vector3 target, Ship ship)
    {
        if (IsPathToTargetClear(start, target, ship))
        {
            var point = new courseWaypoint()
            {
                position = target,
                rotation = ship.transform.rotation,
                velocity = Vector3.zero
            };
            course.Add(point);

            return;
        }
  

        //Blocked!
        Vector3 offset = target - start;
        Vector3 midway = start + (offset * 0.5f);

       //we know we cant fly direct from midway to target, so lets look at some places around midway
       //this should give us minimum deflection
        var options = new Vector3[18];
        options[0] = new Vector3(midway.x + (pathRadius * 2f), midway.y, midway.z);
        options[1] = new Vector3(midway.x - (pathRadius * 2f), midway.y, midway.z);
        options[2] = new Vector3(midway.x, midway.y + (pathRadius * 2f), midway.z);
        options[3] = new Vector3(midway.x, midway.y - (pathRadius * 2f), midway.z);
        options[4] = new Vector3(midway.x, midway.y, midway.z + (pathRadius * 2f));
        options[5] = new Vector3(midway.x, midway.y, midway.z - (pathRadius * 2f));
        options[6] = new Vector3(midway.x + (pathRadius * 5f), midway.y, midway.z);
        options[7] = new Vector3(midway.x - (pathRadius * 5f), midway.y, midway.z);
        options[8] = new Vector3(midway.x, midway.y + (pathRadius * 5f), midway.z);
        options[9] = new Vector3(midway.x, midway.y - (pathRadius * 5f), midway.z);
        options[10] = new Vector3(midway.x, midway.y, midway.z + (pathRadius * 5f));
        options[11] = new Vector3(midway.x, midway.y, midway.z - (pathRadius * 5f));
        options[12] = new Vector3(midway.x + (pathRadius * 10f), midway.y, midway.z);
        options[13] = new Vector3(midway.x - (pathRadius * 10f), midway.y, midway.z);
        options[14] = new Vector3(midway.x, midway.y + (pathRadius * 10f), midway.z);
        options[15] = new Vector3(midway.x, midway.y - (pathRadius * 10f), midway.z);
        options[16] = new Vector3(midway.x, midway.y, midway.z + (pathRadius * 10f));
        options[17] = new Vector3(midway.x, midway.y, midway.z - (pathRadius * 10f));

        for(int i=0;i< 18;i++)
        {
            var option = options[i];

            if(IsPathToTargetClear(start, option, ship) && IsPathToTargetClear(option,target,ship))
            {
                //OK, we can go from here to the point "option" and then from there to the target direct
                PlotCourseVia(start, target, option, ship);

                return;
            }
        }

        //huh ... blocked a lot around the midway, 
        //try diverting around the target, when we'll be slowing down anyway, so  a greater deflection is less significant
        options[0] = new Vector3(target.x + (pathRadius * 2f), target.y, target.z);
        options[1] = new Vector3(target.x - (pathRadius * 2f), target.y, target.z);
        options[2] = new Vector3(target.x, target.y + (pathRadius * 2f), target.z);
        options[3] = new Vector3(target.x, target.y - (pathRadius * 2f), target.z);
        options[4] = new Vector3(target.x, target.y, target.z + (pathRadius * 2f));
        options[5] = new Vector3(target.x, target.y, target.z - (pathRadius * 2f));
        options[6] = new Vector3(target.x + (pathRadius * 5f), target.y, target.z);
        options[7] = new Vector3(target.x - (pathRadius * 5f), target.y, target.z);
        options[8] = new Vector3(target.x, target.y + (pathRadius * 5f), target.z);
        options[9] = new Vector3(target.x, target.y - (pathRadius * 5f), target.z);
        options[10] = new Vector3(target.x, target.y, target.z + (pathRadius * 5f));
        options[11] = new Vector3(target.x, target.y, target.z - (pathRadius * 5f));
        options[12] = new Vector3(target.x + (pathRadius * 10f), target.y, target.z);
        options[13] = new Vector3(target.x - (pathRadius * 10f), target.y, target.z);
        options[14] = new Vector3(target.x, target.y + (pathRadius * 10f), target.z);
        options[15] = new Vector3(target.x, target.y - (pathRadius * 10f), target.z);
        options[16] = new Vector3(target.x, target.y, target.z + (pathRadius * 10f));
        options[17] = new Vector3(target.x, target.y, target.z - (pathRadius * 10f));

        for (int i = 0; i < 18; i++)
        {
            var option = options[i];

            if (IsPathToTargetClear(start, option, ship) && IsPathToTargetClear(option, target, ship))
            {
                PlotCourseVia(start, target, option, ship);

                return;
            }
        }

        //huh ... blocked a lot around the midway, and around the target, maybe the blockage is closer to our start
        // lets try diverting around our starting postion
        options[0] = new Vector3(start.x + (pathRadius * 2f), start.y, start.z);
        options[1] = new Vector3(start.x - (pathRadius * 2f), start.y, start.z);
        options[2] = new Vector3(start.x, start.y + (pathRadius * 2f), start.z);
        options[3] = new Vector3(start.x, start.y - (pathRadius * 2f), start.z);
        options[4] = new Vector3(start.x, start.y, start.z + (pathRadius * 2f));
        options[5] = new Vector3(start.x, start.y, start.z - (pathRadius * 2f));
        options[6] = new Vector3(start.x + (pathRadius * 5f), start.y, start.z);
        options[7] = new Vector3(start.x - (pathRadius * 5f), start.y, start.z);
        options[8] = new Vector3(start.x, start.y + (pathRadius * 5f), start.z);
        options[9] = new Vector3(start.x, start.y - (pathRadius * 5f), start.z);
        options[10] = new Vector3(start.x, start.y, start.z + (pathRadius * 5f));
        options[11] = new Vector3(start.x, start.y, start.z - (pathRadius * 5f));
        options[12] = new Vector3(start.x + (pathRadius * 10f), start.y, start.z);
        options[13] = new Vector3(start.x - (pathRadius * 10f), start.y, start.z);
        options[14] = new Vector3(start.x, start.y + (pathRadius * 10f), start.z);
        options[15] = new Vector3(start.x, start.y - (pathRadius * 10f), start.z);
        options[16] = new Vector3(start.x, start.y, start.z + (pathRadius * 10f));
        options[17] = new Vector3(start.x, start.y, start.z - (pathRadius * 10f));

        for (int i = 0; i < 18; i++)
        {
            var option = options[i];

            if (IsPathToTargetClear(start, option, ship) && IsPathToTargetClear(option, target, ship))
            {
                PlotCourseVia(start, target, option, ship);

                return;
            }
        }

        //can we fly straight to the midway point?
        if (IsPathToTargetClear(start, midway, ship))
        {
            // yes, plot a course to the midway point and work our way from there
            PlotCourseVia(start, target, midway, ship);

            return;
        }

        //Diverting from the midway point, from the start and from the target are all a no-go.  Am I blocked?
        Debug.LogWarningFormat("PilotIntercepter {0} cant find a route to {1}", ship.name, ship.target.name);
        Time.timeScale = 0f; //Pause the game

        //TODO devise a fallback
    }

    private void PlotCourseVia(Vector3 start, Vector3 target, Vector3 midway, Ship ship)
    {
        // yes, plot a course to the midway point and work our way from there
        var point = new courseWaypoint()
        {
            position = target,
            rotation = ship.transform.rotation,
            velocity = ship.GetComponent<Rigidbody>().velocity * 0.5f
        };
        course.Add(point);

        PlotCourseTo(midway, target, ship);
    }

    private bool IsPathToTargetClear(Vector3 start, Vector3 target, Ship ship)
    {

        Vector3 direction = target - start;
        float distance = direction.magnitude;
        direction.Normalize();
        RaycastHit hits;

        if (Physics.SphereCast(start, pathRadius, direction, out hits, distance) == false)
        {
            return true; //didn't hit anything
        }

        if (hits.collider.gameObject == ship.gameObject)
        {
            Debug.LogFormat("PilotInterceptor {0} cast hit itself", ship.name);
            return true;
        }

        if (hits.collider.gameObject == ship.target)
        {
            return true;
        }

        return false;
    }

    public class courseWaypoint
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
    }
}