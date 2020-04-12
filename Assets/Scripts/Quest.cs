using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class QuestEvent : UnityEvent<Quest> { };

public class Quest : MonoBehaviour {
    public enum Status
    {
        pending,
        active,
        succeded,
        failed
    }

    public string text;
    public bool beginAtStart;
    public bool required;
    public Status status = Status.pending;
    public GameObject waypoint;
    public Quest[] subQuests;

    public QuestEvent OnBegin = new QuestEvent();
    public QuestEvent OnSuccess = new QuestEvent();
    public QuestEvent OnFail = new QuestEvent();

    private void Start()
    {
        if (beginAtStart) Begin();
    }

    public void Begin()
    {
        status = Status.active;

        //if(waypoint != null)
        //{
        //    var wpComp = waypoint.GetComponent<PointOfInterest>();
        //    wpComp.tag = wpComp.tag + ",Quest";
        //}

        //if we have sub-quests lets wire up some events so things progress as desired
        InitialiseSubQuestEvents();

        Debug.LogFormat("Quest {0} begins", text);

        if (OnBegin != null) OnBegin.Invoke(this);
    }

    public void Begin(Quest other)
    {
        Begin();
    }

    public void Success()
    {
        status = Status.succeded;

        Debug.LogFormat("Quest {0} succeded", text);

        if (OnSuccess != null) OnSuccess.Invoke(this);
    }

    public void Success(Quest other)
    {
        Success();
    }

    public void Fail()
    {
        status = Status.failed;

        Debug.LogFormat("Quest {0} failed", text);

        if (OnFail != null) OnFail.Invoke(this);
    }

    public void Fail(Quest other)
    {
        Fail();
    }

    private void InitialiseSubQuestEvents()
    {
        if (subQuests == null || subQuests.Length == 0) return;

        Quest previous = null;
        Quest lastSubQuest = subQuests.Last();
        foreach (Quest Current in subQuests)
        {
            if (previous != null)
            {
                if (previous.required)
                {
                    previous.OnSuccess.AddListener(Current.Begin);
                    previous.OnFail.AddListener(this.Fail);
                }
                else
                {
                    previous.OnSuccess.AddListener(Current.Begin);
                    previous.OnFail.AddListener(Current.Begin);
                }
            }

            if (Current == lastSubQuest)
            {
                Current.OnSuccess.AddListener(this.Success);
                if (Current.required)
                {
                    Current.OnFail.AddListener(this.Success);
                }
                else
                {
                    Current.OnFail.AddListener(this.Success);
                }
            }

            previous = Current;
        }

        OnBegin.AddListener(subQuests[0].Begin);
    }
}
