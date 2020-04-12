using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Utility/Publication")]
public class Publication : ScriptableObject
{
    public List<Subscriber> subscribers = new List<Subscriber>();

    public void Subscribe(Subscriber subscriber)
    {
        if (subscribers.Contains(subscriber))
            return;

        subscribers.Add(subscriber);
    }
    public void UnSubscribe(Subscriber subscriber)
    {
        if (!subscribers.Contains(subscriber))
            return;

        subscribers.Remove(subscriber);
    }

    public void Publish()
    {
        Debug.Log("Publishing " + this.name);

        if (subscribers.Count == 0)
            return;

        for (int index = subscribers.Count - 1; index >= 0; index--)
        {
            Debug.Log("invoking " + subscribers[index].name);
            subscribers[index].onPublish.Invoke();
        }        
    }
}