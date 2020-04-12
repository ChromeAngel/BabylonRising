using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuButtonUI : MonoBehaviour {
    private Image backgroundImage;
    private Text foregroundText;

	// Use this for initialization
	void Start () {
        backgroundImage = GetComponent<Image>();
        foregroundText = GetComponentInChildren<Text>();
    }

    private bool _isEnabled = true;
	
    public bool IsEnabled
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            _isEnabled = value;
            Color c = foregroundText.color;

            if (_isEnabled)
            {
                c.a = 0.9f;
            } else
            {
                c.a = 0.3f;
            }

            foregroundText.color = c;
        }
    }

    private bool _isCurrent = true;

    public bool IsCurrent
    {
        get
        {
            return _isCurrent;
        }
        set
        {
            _isCurrent = value;
            Color c = backgroundImage.color;

            if(_isCurrent)
            {
                backgroundImage.color = HUD.colors.pct60(HUD.colors.HUD);
            } else
            {
                backgroundImage.color = HUD.colors.pct20(HUD.colors.HUD);
            }
        }
    }

    public void InitialiseButton(int index, float buttonArc, float buttonArcDegrees, RadialMenu.MenuItem m, int buttonCount)
    {
       // Debug.LogFormat("Initialising button {0} of {1}", index, buttonCount);
        float arcedDegrees = buttonArcDegrees * index; //offset at which the button arc starts
        float centerDegrees = buttonArcDegrees * 0.5f; //half way around the arc a button covers
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, RadialMenu.normalizeAngle(arcedDegrees));
        name = string.Format("{0}RadialMenuButton", m.text);

        Transform arm = transform.Find("Arm");

        if (arm != null)
        {
            arm.localRotation = Quaternion.Euler(0f, 0f, RadialMenu.normalizeAngle(360f - centerDegrees));
        }

        
        foregroundText = GetComponentInChildren<Text>();
        foregroundText.text = m.text;

        //Debug.LogFormat("Initialising button {0} text at {1} + ({2} * ({3} - {4})", index, centerDegrees , arcedDegrees, buttonCount, index);

        foregroundText.transform.localRotation = Quaternion.Euler(0f, 0f, RadialMenu.normalizeAngle(centerDegrees + (buttonArcDegrees * (buttonCount - index))));

        backgroundImage = GetComponent<Image>();
        backgroundImage.fillAmount = buttonArc;
        backgroundImage.color = HUD.colors.pct20(HUD.colors.HUD);

        IsEnabled = m.enabled;
        IsCurrent = false;
    }

    public Vector2 GetTextOrigin()
    {
        return foregroundText.rectTransform.position;
    }
}
