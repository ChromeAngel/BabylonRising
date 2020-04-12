using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Predictable : MonoBehaviour {
    private Rigidbody body;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();

        if(_radius == 0f)
        {
            var sphereCollider = GetComponent<SphereCollider>();
            if(sphereCollider != null)
            {
                _radius = sphereCollider.radius;
            }
        }

        if(Oracle.Instance) Oracle.Instance.Subscribe(this);
    }
	
    private void OnDestroy()
    {
#if UNITY_EDITOR
        if(UnityEditor.EditorApplication.isPlaying)
        {
            if (Oracle.Instance) Oracle.Instance.UnSubscribe(this);
        }
#else
       if(Oracle.Instance) Oracle.Instance.UnSubscribe(this);
#endif
    }

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    public Vector3 velocity
    {
        get
        {
            return body.velocity;
        }
    }

    private float _timeOfDeath;
    public float TimeOfDeath
    {
        get
        {
            return _timeOfDeath;
        }
        set
        {
            _timeOfDeath = value;
            Oracle.Instance.OnBodyChanged(this);
        }
    }

    [SerializeField]
    private float _radius = 0f;

    public float Radius
    {
        get
        {
            return _radius;
        }
        set
        {
            _radius = value;
            Oracle.Instance.OnBodyChanged(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Oracle.Instance.OnBodyChanged(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
