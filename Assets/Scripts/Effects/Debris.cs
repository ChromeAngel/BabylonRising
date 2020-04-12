using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Debris : MonoBehaviour {
    public GameObject[] debrisPrefabs;
    public ParticleSystem[] particles;

    private Chance _chance;

    private void Start()
    {
        if(debrisPrefabs == null || debrisPrefabs.Length == 0)
        {
            Debug.LogWarningFormat("Debris {0} has no prefabs specified", name);

            return;
        }

        _chance = GetComponent<Chance>();

        Vector3 origin = transform.position;

        int chunks = Mathf.CeilToInt(transform.localScale.magnitude / 2f);
        for (int i = 0; i < chunks; i++)
        {
            CreateDebris(origin);
        }

        if (particles == null || particles.Length == 0) return;

        foreach(var ps in particles)
        {
            ps.Play();
        }
        
    }

    private void CreateDebris(Vector3 origin)
    {
        Vector3 offset = transform.localScale * 6f;
        offset.x = _chance.Random(1f) * offset.x;
        offset.y = _chance.Random(1f) * offset.y;
        offset.z = _chance.Random(1f) * offset.z;

        Vector3 chunkOrigin = origin + offset;

        GameObject prefab = _chance.Pick<GameObject>(debrisPrefabs);
        GameObject chunk = GameObject.Instantiate(prefab, chunkOrigin, Quaternion.Euler(_chance.Vector(360f)), transform.parent);

        Rigidbody crb = chunk.GetComponent<Rigidbody>();

        if (crb == null) crb = chunk.AddComponent<Rigidbody>();

        chunk.AddComponent<OutOfSightOutOfMind>();

        //spin 'em
        crb.angularVelocity = _chance.Vector(6f); //6f ~ 2 pi or 1 rotation per second, spinnign bigger rocks more slowly

        crb.useGravity = false;

        crb.mass = 2f;
        crb.AddExplosionForce(1f, origin, transform.localScale.magnitude);
    }
}
