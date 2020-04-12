using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class QuestLog : MonoBehaviour {
    public GameObject logEntryPrefab;

    public AudioSource questBegin;
    public AudioSource questPass;
    public AudioSource questFail;

    private List<QuestLogEntry> logEntries;
    private float yOffset = 0f;
    //private QuestLogEntry activeLogEntry;
	// Use this for initialization
	void Awake () {
        logEntries = new List<QuestLogEntry>();

        var quests = GameObject.FindObjectsOfType<Quest>();

        Debug.LogFormat("QuestLog identified {0} quests", quests.Length);

        foreach(Quest subject in quests)
        {
            subject.OnBegin.AddListener(OnQuestBegin);
            subject.OnSuccess.AddListener(OnQuestSucceded);
            subject.OnFail.AddListener(OnQuestFailed);
        }
	}

    public void OnQuestBegin(Quest subject)
    {
        Debug.LogFormat("QuestLog quest {0} begins", subject.text);
        GameObject goLogEntry = GameObject.Instantiate(logEntryPrefab);
        if (goLogEntry == null)
        {
            Debug.LogWarning("QuestLog failed to create a new log entry");
            return;
        }

        RectTransform logRT = goLogEntry.GetComponent<RectTransform>();

        if (logRT == null)
        {
            Debug.LogWarning("QuestLog entry has no RectTransform component");
            return;
        }

        logRT.SetParent(transform);
        logRT.localPosition = new Vector3(0f, yOffset, 0f); //TODO x offset sub quests
        logRT.localScale = Vector3.one;
        yOffset -= logRT.rect.height;

        QuestLogEntry logNtry = goLogEntry.GetComponent<QuestLogEntry>();

        if (logNtry == null)
        {
            Debug.LogWarning("QuestLog entry has no QuestLogEntry component");
            return;
        }

        logNtry.quest = subject;

        goLogEntry.SetActive(true);

        logEntries.Add(logNtry);

        if(questBegin != null)
        {
            if(! questBegin.isPlaying) questBegin.Play();
        }
      //  activeLogEntry = logNtry;
    }

    public void OnQuestSucceded(Quest subject)
    {
        if (questPass != null)
        {
            if (!questPass.isPlaying) questPass.Play();
        }

        var entry = logEntries.FirstOrDefault(x => x.quest == subject);
        if (entry == null) return;
    }

    public void OnQuestFailed(Quest subject)
    {
        if (questFail != null)
        {
            if (!questFail.isPlaying) questFail.Play();
        }

        var entry = logEntries.FirstOrDefault(x => x.quest == subject);
        if (entry == null) return;
    }

    public void ReflowLogEntries()
    {
        var deadies = new List<QuestLogEntry>();
        yOffset = 0f;
        foreach(QuestLogEntry e in logEntries)
        {
            if(e.gameObject.activeInHierarchy)
            {
                RectTransform logRT = e.GetComponent<RectTransform>();

                if (logRT == null)
                {
                    deadies.Add(e);
                }

                logRT.SetParent(transform);
                logRT.localPosition = new Vector3(0f, yOffset, 0f); //TODO x offset sub quests
                logRT.localScale = Vector3.one;
                yOffset -= logRT.rect.height;
            } else
            {
                deadies.Add(e);
            }
        }
        foreach(QuestLogEntry e in deadies)
        {
            logEntries.Remove(e);
            e.quest = null;
            GameObject.Destroy(e.gameObject);
        }

        System.GC.Collect();
    }
}
