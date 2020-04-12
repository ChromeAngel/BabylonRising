using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;


//Removes itself  as soon as the player a stops looking at it, great for debris
public class OutOfSightOutOfMind : MonoBehaviour {
    private float nextThink;
    private Transform _camera;
    // Use this for initialization
    void Start()
    {
        nextThink = Time.time + 0.5f;

        var goPlayer = GameObject.FindGameObjectWithTag("Player");
        _camera = goPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextThink < Time.time) return;

        //Camera main = Camera.current;

        //Vector3 viewPos = _camera.WorldToViewportPoint(transform.position);

        Vector3 cameraforward = _camera.TransformDirection(Vector3.forward);
        var vectorToItem = (transform.position - _camera.position);
        //float angleFromCamera = Vector3.Angle(vectorToItem, _camera.forward);

        //if (angleFromCamera > 90f) //It's behind us
        //{
        //    Debug.LogFormat("OutOfSightOutOfMind cleaning up {0}", gameObject.name);
        //    gameObject.SetActive(false);
        //    Destroy(gameObject);
        //}
        if (Vector3.Dot(cameraforward, vectorToItem) < 0 ) //It's behind us
        {
            Debug.LogFormat("OutOfSightOutOfMind cleaning up {0}", gameObject.PathID());
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
           // Debug.LogFormat("OSOM Update {0} z={1}", gameObject.name, angleFromCamera);
            nextThink = Time.time + 0.5f;
        }
    }
}
