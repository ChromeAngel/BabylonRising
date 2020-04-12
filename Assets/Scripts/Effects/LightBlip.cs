using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightBlip : MonoBehaviour {

    private Light _light = null;
    private Color initialColor;
    private Color fadedColor;
    private float StartTime;
    private float EndTime;
    private float secondsDuration;

    public void Blip(float secondsDuration)
    {
        this.secondsDuration = secondsDuration;
        StartTime = Time.time;
        EndTime = Time.time + secondsDuration;
    }

    private void Start()
    {
        _light = GetComponent<Light>();
    //    _light.enabled = false;

        initialColor = _light.color;
        fadedColor = _light.color;
        fadedColor.a = 0f;
        StartTime = float.MaxValue;
        EndTime = float.MaxValue;
    }

    // Update is called once per frame
	void Update () {
        if (Time.time < StartTime) return; 
        if(Time.time > EndTime)
        {
       //     _light.enabled = false;
            gameObject.SetActive(false);
            GameObject.Destroy(gameObject);

            return;
        }

       // _light.enabled = true;
        float elapsed = Time.time - StartTime;
        float ratio = 1f - (elapsed / secondsDuration);

        Debug.LogFormat("Lighting ratio {0}", ratio);

        _light.color = Color.Lerp(initialColor, fadedColor, ratio);

    }
}
