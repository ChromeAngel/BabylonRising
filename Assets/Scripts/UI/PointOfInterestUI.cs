using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 shape : ship, base, cargo, waypoint
 color : friendly, neutral, hostile
 scale : emphasis? importance? range?
 opacity : emphasis? importance? range?
 rotation
*/
public class PointOfInterestUI : MonoBehaviour {
    private GameObject _subject;
    public GameObject subject {
        get
        {
            return _subject;
        }
    }

    public bool SetSubject(GameObject subject)
    {
        _subject = subject;

        if (_subject == null)
        {
            Debug.LogWarningFormat("PointofIterestUI {0} has a null subject", gameObject.name);

            gameObject.SetActive(false);

            return false;
        }

        Text Glyph = GetComponentInChildren<Text>();

        if (Glyph == null)
        {
            Debug.LogWarningFormat("PointofIterestUI {0} does not have a Text child", gameObject.name);

            return false;
        }

        Glyph.text = GetIcon();

        var HUD = GetComponentInParent<HUD>();
        if (HUD != null)
        {
            Glyph.color = HUD.GetRelativeColor(_subject);
        }

        return true;
    }

    private string GetIcon()
    {
        string icon = "#";

        if (subject.tag.Contains("Starbase"))
        {
            icon = "\"";
        }
        else if (subject.tag.Contains("Collectable"))
        {
            icon = "$";
        }
        else if (subject.tag.Contains("Crew"))
        {
            icon = "%";
        }
        else if (subject.tag.Contains("JumpPoint"))
        {
            icon = "*";
        }
        else if (subject.tag.Contains("WayPoint"))
        {
            icon = "@";
        }

        return icon;
    }
}
