using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class HUDCaption : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        var label = GetComponent<Text>();
        label.text = string.Format("{0} v{1}", Application.productName.ToUpperInvariant(), Application.version);
        label.color = HUD.colors.pct20(HUD.colors.HUD);
	}

    
}
