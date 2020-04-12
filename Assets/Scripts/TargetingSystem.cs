using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text;
using System.Linq;
using ExtensionMethods;

public class TargetingSystem : MonoBehaviour {
    private Canvas HUDCanvas;
    private Camera HUDCam;

    private GameObject _target;
    private GameObject _player;
    private Ship _player_ship;
    private Damageable target_dmg;

    public Text Readout;
    public GameObject target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;

            if(_target == null)
            {
                _player_ship.target = null;
                target_dmg = null;
                Readout.color = HUD.colors.pct20(HUD.colors.HUD);
            } else
            {
                _player_ship.target = _target;

                var HUD = GetComponentInParent<HUD>();
                if (HUD != null)
                {
                    Readout.color = HUD.GetRelativeColor(_target);
                }

                target_dmg = _target.GetComponentInChildren<Damageable>();
            }
            
            if (OnTargetChanged != null) OnTargetChanged.Invoke();
        }
    }

    public UnityEvent OnTargetChanged;

    private Vector2 center;

    /// <summary>
    /// Target the item in the list nearest to the crosshair
    /// </summary>
    /// <param name="list"></param>
    public void TargetNearestAngleFromList(GameObject[] list)
    {
        GameObject Nearest = null;
        float nearestRange = float.MaxValue;

        foreach (var listedObject in list)
        {
            Vector3 viewPos = HUDCam.WorldToViewportPoint(listedObject.transform.position);

            if (viewPos.z < 0) continue;

            Vector2 PPos = new Vector2(viewPos.x, viewPos.y);

            float range = Vector2.Distance(PPos, center);

            if (range < nearestRange)
            {
                nearestRange = range;
                Nearest = listedObject.gameObject;
            }
        }

        target = Nearest;
    } // end TargetNearestAngleFromList

    /// <summary>
    /// target the point of interest nearest the crosshair
    /// </summary>
    public void TargetNearestAngle()
    {
        var PointsOfInterest = GameObject.FindObjectsOfType<PointOfInterest>().Select(
            x => x.gameObject
        ).ToArray();

        TargetNearestAngleFromList(PointsOfInterest);
    } // end TargetNearestAngle


    /// <summary>
    /// Target the quest objective nearest the crosshair
    /// </summary>
    public void TargetNearestQuest()
    {
        var Quests = GameObject.FindObjectsOfType<Quest>().Where(
            x => x.status == Quest.Status.active 
            && x.waypoint != null
        ).Select(
            x => x.waypoint
        ).ToArray();

        TargetNearestAngleFromList(Quests);
    }

    public void EngaugeAutoPilot()
    {
        _player_ship.AI = _player_ship.pilots[(int)AutoPilot.pilots.Intercept];
    }

    private void Start()
    {
        center = new Vector2(0.5f, 0.5f);
        HUDCanvas = GetComponentInParent<Canvas>();
        HUDCam = HUDCanvas.worldCamera;
        _player = GameObject.FindGameObjectWithTag("Player");
        _player_ship = _player.GetComponent<Ship>();

        if (Readout == null) return;

        Readout.color = HUD.colors.pct20(HUD.colors.HUD);
    }

    private void Update()
    {
        if (Readout == null) return;

        var message = new StringBuilder("No Target");

        if(target != null)
        {
            if(target.activeInHierarchy)
            {
                message.Length = 0;

                if (_player != null)
                {
                    //float range = Vector3.Distance(_player.transform.position, _target.transform.position);
                    var angleToTarget = _player.transform.PitchAndYaw(_target.transform);

                    message.AppendFormat("{1:000}°,{2:000}° Range {0}m\n", Mathf.FloorToInt(angleToTarget.z), angleToTarget.x, angleToTarget.y);
                }

                message.AppendLine(target.tag);

                //If it's damageable, show it's health
                if(target_dmg != null)
                {
                    message.AppendFormat("Damage {0}%\n", Mathf.CeilToInt(100f * target_dmg.Value / target_dmg.MaxValue));
                }
            }
            else
            {
                message.Length = 0;
                message.Append("Target Lost");
                Readout.color = HUD.colors.pct60(HUD.colors.Enemy);
            }
        }

        Readout.text = message.ToString();
    }
}
