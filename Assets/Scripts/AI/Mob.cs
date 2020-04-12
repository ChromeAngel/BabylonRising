using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {

    public List<GameObject> members;
    public Dictionary<string, object> memory;

	// Use this for initialization
	void Start () {
        memory = new Dictionary<string, object>();
	}

    // Update is called once per frame
    //void Update () {

    //}

    private Mob _parent;
    public Mob parent
    {
        get
        {
            if(_parent == null)
            {
                var mobs = GameObject.FindObjectsOfType<Mob>();
                foreach(Mob m in mobs)
                {
                    if (m == this) continue;
                    if (m.members != null 
                        && m.members.Contains(gameObject))
                    {
                        _parent = m;

                        break;
                    }
                }
            }

            return _parent;
        }
    }

    public void Remember(string key, object value)
    {
        memory[key] = value;
        memory["lastUpdated"] = Time.time;
    }

    public object Recall(string key)
    {
        if (memory.ContainsKey(key)) return memory[key];
        if (parent == null) return null;

        return parent.Recall(key);
    }

    public void Forget(string key)
    {
        if(memory.ContainsKey(key)) memory.Remove(key);
    }

    public void Join(Mob newParent)
    {
        if(_parent != null)
        {
            _parent.members.Remove(gameObject);
        }

        _parent = newParent;

        if (_parent != null)
        {
            _parent.members.Add(gameObject);
        }
    }

    public bool IsPeer(GameObject other)
    {
        if (parent == null) return false;

        return parent.members.Contains(other);
    }
}
