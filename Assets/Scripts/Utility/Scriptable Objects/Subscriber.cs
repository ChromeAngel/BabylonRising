using UnityEngine;
using UnityEngine.Events;

public class Subscriber : MonoBehaviour
{
    public Publication publication;
    public UnityEvent onPublish = new UnityEvent();
    private void OnEnable()
    {
        if (publication == null)
            return;

        publication.Subscribe(this);
    }

    private void OnDisable()
    {
        if (publication == null)
            return;

        publication.UnSubscribe(this);
    }
}
