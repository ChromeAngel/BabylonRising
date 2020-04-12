using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class QuestLogEntry : MonoBehaviour {
    public Quest quest;
    public UnityEvent OnFinished;

    private Text txt;
    private Animator anm;

    void Start () {
        if (quest == null)
        {
            Debug.LogWarning("QuestLogEntry has no Quest");
            return;
        }

        quest.OnSuccess.AddListener(OnQuestSucceded);
        quest.OnFail.AddListener(OnQuestFailed);

        txt = GetComponent<Text>();

        if (txt == null)
        {
            Debug.LogWarning("QuestLogEntry has no Text component");
            return;
        }

        txt.text = quest.text;
        if (quest.required)
        {
            txt.color = HUD.colors.pct80(HUD.colors.Quest);
        }
        else
        {
            txt.color = HUD.colors.pct60(HUD.colors.Quest);
        }

        Animator anm = GetComponent<Animator>();
        if (anm != null)
        {
            anm.SetBool("IsLive", true);
        }
    }

    public void OnQuestSucceded(Quest subject)
    {
        if (txt != null)
        {
            txt.color = HUD.colors.pct40(HUD.colors.Ally);
        }

        Animator anm = txt.GetComponent<Animator>();
        if (anm != null)
        {
            anm.SetBool("IsLive", false);
        }
    }

    public void OnQuestFailed(Quest subject)
    {
        if (txt != null)
        {
            txt.color = HUD.colors.pct40(HUD.colors.Enemy);
        }

        Animator anm = txt.GetComponent<Animator>();
        if (anm != null)
        {
            anm.SetBool("IsLive", false);
        }
    }

    public void Finished()
    {
        gameObject.SetActive(false);

        if (OnFinished != null) OnFinished.Invoke();
    }
}
