using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ExtensionMethods;



public class Oracle : Tayx.Graphy.Utils.G_Singleton<Oracle>
{

    //private static Oracle _instance = null;

    //public static Oracle instance()
    //{
    //    if(_instance == null)
    //    {
    //        var Auto = new GameObject();
    //        Auto.name = "AutomaticOracleSingleton";
    //        _instance = Auto.AddComponent<Oracle>();
    //    }

    //    return _instance;
    //}

	// Use this for initialization
	void Start () {
        //if (_instance == null)
        //{
        //    _instance = this;
        //} else
        //{
        //    if(_instance != this)
        //    {
        //        Debug.LogWarningFormat("Duplicate Oracle removing itself {0}", gameObject.PathID());
        //        gameObject.SetActive(false);
        //        Destroy(gameObject);

        //        return;
        //    }
        //}

        cache = new Dictionary<float, Prediction>();
    }

    //All the things we're going to try and predicta about
    private List<Predictable> bodies = new List<Predictable>();

    // what we predict for a given body
    public struct Predicted
    {
        public GameObject gameObject;
        public Vector3 position;
        public float radius;
    }

    //what we predict for all bodies at a given time
    public struct Prediction
    {
        public float time;
        public Predicted[] bodies;
    }

    //a cache of all our predictions, so we don't duplicate work
    private Dictionary<float, Prediction> cache = new Dictionary<float, Prediction>();

    /// <summary>
    /// Predictables notify us that they can be predicted
    /// </summary>
    /// <param name="item">a body to be predicted</param>
    public void Subscribe(Predictable item)
    {
        bodies.Add(item);
        cache.Clear();
    }

    /// <summary>
    /// Predictables notify us that they can no longer be predicted
    /// </summary>
    /// <param name="item"></param>
    public void UnSubscribe(Predictable item)
    {
        bodies.Remove(item);
        cache.Clear();
    }

    /// <summary>
    /// A body can notify us that it's behaviour has changed, invalidating our predictions
    /// </summary>
    /// <param name="item"></param>
    public void OnBodyChanged(Predictable item)
    {
        cache.Clear();
    }

    /// <summary>
    /// Predict the state of all known bodies some time in the future
    /// </summary>
    /// <param name="timeOffset">how many seconds into the future we need to predict</param>
    /// <returns>state of the predictable bodies in the future</returns>
    public Prediction Predict(float timeOffset)
    {
        float predictedTime = Time.time + timeOffset;

        if (cache.ContainsKey(predictedTime)) return cache[predictedTime];

        var result = new Prediction() {
            time = predictedTime,
            bodies = this.bodies
                .Where(x => x.TimeOfDeath >= predictedTime)
                .Select(y => new Predicted()
                {
                    gameObject = y.gameObject,
                    position = y.position + (y.velocity * timeOffset),
                    radius = y.Radius
                })
                .ToArray()
        };

        cache.Add(predictedTime, result);

        return result;
    }

    public struct CollisionPrediction
    {
        public GameObject other;
        public float collisionTime;
        public Vector3 collisionPosition;
    }

    /// <summary>
    /// check for collsions between me and anything else within a timeframe
    /// </summary>
    /// <param name="me">a predictable who we are going to check for collsions against</param>
    /// <param name="TimeOffset">how many seconds in the future we're going to look</param>
    /// <returns>a CollisionPrediction with a null other if no collsion is predicted, else the gameobject collided with and time and place of where that is predicted to happen</returns>
    public CollisionPrediction PredictCollison(Predictable me, float TimeOffset)
    {
        CollisionPrediction result = new CollisionPrediction();
        Prediction startState = Predict(0f);
        Prediction endState = Predict(TimeOffset);
        float frameSeconds = 1f / 60f;
        //interpoltate between our start and end states every 60th of a second
        for(float frameTime = 0; frameTime < TimeOffset; frameTime = frameTime + frameSeconds)
        {
            float lerpTime = startState.time + frameTime;
            Prediction lerpState;

            //check the cache in case it's been predicted already (unlikely)
            if (cache.ContainsKey(lerpTime))
            {
                lerpState = cache[lerpTime];
            } else
            {
                lerpState = new Prediction() { time = lerpTime };
                var lerpBodies = new List<Predicted>();
                foreach(Predicted startBody in startState.bodies)
                {
                    Predicted endBody = endState.bodies.FirstOrDefault(x => x.gameObject == startBody.gameObject);

                    if (endBody.gameObject == null) continue;

                    var lerpedBody = new Predicted()
                    {
                        gameObject = startBody.gameObject,
                        position = Vector3.Lerp(startBody.position, endBody.position, lerpTime / TimeOffset),
                        radius = Mathf.Lerp(startBody.radius, endBody.radius, lerpTime / TimeOffset)
                    };

                    lerpBodies.Add(lerpedBody);
                }
                lerpState.bodies = lerpBodies.ToArray();
            }

            //find the state of "me" in this frame
            var lerpedMe = lerpState.bodies.FirstOrDefault(x => x.gameObject == me.gameObject);

            //check for collsions of me against all the other bodies in this frame's state
            foreach(Predicted other in lerpState.bodies)
            {
                if (other.gameObject == lerpedMe.gameObject) continue;

                float distance = Vector3.Distance(other.position, lerpedMe.position);

                if (distance > other.radius + lerpedMe.radius) continue;

                result.other = other.gameObject;
                result.collisionTime = lerpTime;
                result.collisionPosition = lerpedMe.position;

                return result;
            }
        
        } // next frame

        result.other = null;

        return result;
    }
}
