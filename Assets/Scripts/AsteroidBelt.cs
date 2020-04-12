using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chance))]
public class AsteroidBelt : MonoBehaviour {
    public float minScale;
    public float maxScale;
    public float asteroidDensity;
    public int instances;

    public GameObject[] prefabs;

    public Transform targetPoint;

    private Chance Chance;

	// Use this for initialization
	void Start () {
        Chance = GetComponent<Chance>();

        generate();
	}
	
	public void generate()
    {
        float[] scales = Chance.StandardDistributionSet(instances, minScale, maxScale);

        //Debug.LogFormat("StandardDistributionSet of {0} items from {1} to {2}:", instances, minScale, maxScale);
        //foreach(float s in scales)
        //{
        //    Debug.Log(s.ToString());
        //}

        Vector3 diffOffset = (targetPoint.position - transform.position) / instances;
        float spreadRadius = diffOffset.magnitude * 2f;
        Vector3 spreadVector = new Vector3(spreadRadius, spreadRadius, spreadRadius);
        Vector3 diffOffsetMin = (diffOffset * 0.25f) - spreadVector;
        Vector3 diffOffsetMax = diffOffset + spreadVector;
        Vector3 lastPosition = transform.position;

        for (int i = 0; i < instances; i++)
        {
            MakeAsteroid(scales[i], diffOffsetMin, diffOffsetMax, lastPosition);

            lastPosition = lastPosition + diffOffset;
        }
    }

    private void MakeAsteroid(float scale, Vector3 diffOffsetMin, Vector3 diffOffsetMax, Vector3 lastPosition)
    {
        Vector3 position = lastPosition;
        position.x += Chance.Random(diffOffsetMin.x, diffOffsetMax.x);
        position.y += Chance.Random(diffOffsetMin.y, diffOffsetMax.y);
        position.z += Chance.Random(diffOffsetMin.y, diffOffsetMax.y);

        Vector3 aScale;
        aScale.x = scale;
        aScale.y = Chance.Random(scale * 0.6f, scale * 1.4f);
        aScale.z = Chance.Random(scale * 0.6f, scale * 1.4f);

        Vector3 rot = Chance.Vector(359f);

        GameObject Asteroid = GameObject.Instantiate(Chance.Pick(prefabs));

        Asteroid.transform.position = position;
        Asteroid.transform.localScale = aScale;
        Asteroid.transform.rotation = Quaternion.Euler(rot);
        Asteroid.transform.SetParent(transform);

        Rigidbody AsteroidBody = Asteroid.GetComponent<Rigidbody>();
        if (AsteroidBody == null)
        {
            AsteroidBody = Asteroid.AddComponent<Rigidbody>();
            AsteroidBody.useGravity = false;
        }

        AsteroidBody.mass = asteroidDensity * aScale.magnitude;

        if(Chance.Random(100) < 30)
        {
            Vector3 spin = Chance.Vector(6f / aScale.magnitude); //6f ~ 2 pi or 1 rotation per second, spinnign bigger rocks more slowly

            AsteroidBody.angularVelocity = spin;
        }

    }
}
