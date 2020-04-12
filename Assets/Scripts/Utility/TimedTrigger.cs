using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using ExtensionMethods;


public class TimedTrigger : MonoBehaviour {
    public UnityEngine.Events.UnityEvent OnTimeout;

    private class TimedItem
    {
        public float triggerTime;
        public float waitSeconds;
        public UnityEvent output;
        public bool repeat;
    }

    private List<TimedItem> queuedItems;

    private void Enqueue(TimedItem item)
    {
        if (item == null) return;
        if (queuedItems == null) queuedItems = new List<TimedItem>();
        if (item.output == null) item.output = OnTimeout;

        queuedItems.Add(item);
    }

    /// <summary>
    /// Wait for a number of seconds then fire the OnTimeout event
    /// </summary>
    /// <param name="secondsWait">how many seconds to wait, zero or less will fire next frame</param>
    public void Wait(float secondsWait)
    {
        Wait(secondsWait, OnTimeout);
    }

    /// <summary>
    /// Wait for a number of seconds then fire the output event
    /// </summary>
    /// <param name="secondsDelay">how many seconds to wait, zero or less will fire next frame</param>
    /// <param name="output">The event to be triggered in secondsDelay</param>
    public void Wait(float secondsDelay, UnityEvent output)
    {
        Enqueue(
            new TimedItem()
            {
                triggerTime = Time.time + secondsDelay,
                waitSeconds = secondsDelay,
                output = output,
                repeat = false
            }
        );
        Debug.LogFormat("{0} is delaying for {1} seconds", this.PathID(), secondsDelay);
    }

    /// <summary>
    /// Fire the OnTimeout event every few seconds
    /// </summary>
    /// <param name="secondsDelay">how many seconds to wait between events, zero or less will fire every frame</param>
    public void Repeat(float secondsDelay)
    {
        Repeat(secondsDelay, OnTimeout);
    }

    /// <summary>
    /// Fire a specified event every few seconds
    /// </summary>
    /// <param name="secondsDelay">how many seconds to wait between events, zero or less will fire every frame</param>
    /// <param name="output">The event to be triggered every secondsDelay</param>
    public void Repeat(float secondsDelay, UnityEvent output)
    {
        Enqueue(
            new TimedItem()
            {
                triggerTime = Time.time + secondsDelay,
                waitSeconds = secondsDelay,
                output = output,
                repeat = true
            }
        );

        Debug.LogFormat("{0} is repeating every {1} seconds", this.PathID(), secondsDelay);
    }

    /// <summary>
    /// Fire the OnTimeout event at a specified time
    /// </summary>
    /// <param name="time">how many seconds after the game started to fire the event</param>
    public void Schedule(float time)
    {
        Schedule(time, OnTimeout);
    }

    /// <summary>
    /// Fire an event at a specified time
    /// </summary>
    /// <param name="time">how many seconds after the game started to fire the event</param>
    /// <param name="output">The event to be triggered at time</param>
    public void Schedule(float time, UnityEvent output)
    {
        Enqueue(
            new TimedItem()
            {
                triggerTime = time,
                waitSeconds = 0f,
                output = output,
                repeat = false
            }
        );

        Debug.LogFormat("{0} is waiting for {1} ", this.PathID(), time);
    }

    // Update is called once per frame, checks for events that are due to be triggered
    void Update()
    {
        if (queuedItems == null) return;
        if (queuedItems.Count == 0) return;

        var pastTimes = queuedItems.Where(t => t.triggerTime <= Time.time).OrderBy(t=>t.triggerTime).ToList();

        foreach (TimedItem Item in pastTimes)
        {
            if (Item.output != null)
            {
                Debug.LogFormat("{0} is firing an event", this.PathID());

                Item.output.Invoke();
            }

            if(Item.repeat)
            {
                Item.triggerTime = Time.time + Item.waitSeconds;
            } else
            {
                queuedItems.Remove(Item);
            }  
        }// end for
    }// end update
}
