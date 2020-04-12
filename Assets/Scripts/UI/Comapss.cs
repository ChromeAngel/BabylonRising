using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Heads Up Display component for highlighting the way to Points Of Interest
/// </summary>
/// 
[RequireComponent(typeof(AudioSource))]
public class Comapss : MonoBehaviour {

    public GameObject pointerUI;

    private List<PointOfInterestUI> UIs;
    private List<PointOfInterestUI> TrashUIs;
    private Canvas HUDCanvas;
    private Camera HUDCam;

    public UnityEvent OnWaypointsChanged;

    private void Start()
    {
        HUDCanvas = GetComponentInParent<Canvas>();
        HUDCam = HUDCanvas.worldCamera;

        Rect screenBounds = HUDCanvas.GetComponent<RectTransform>().rect;

        UIs = new List<PointOfInterestUI>();
        TrashUIs = new List<PointOfInterestUI>();
        RefreshPOIList();
    }

    // Update is called once per frame
    void Update () {
        RectTransform rt = GetComponent<RectTransform>();
        Rect screenBounds = rt.rect;
        TrashUIs.Clear();

        foreach (PointOfInterestUI UI in UIs)
        {
            if(UI.gameObject != null && UI.gameObject.activeInHierarchy && UI.subject != null && UI.subject.activeInHierarchy)
            {
                Vector3 viewPos = HUDCam.WorldToViewportPoint(UI.subject.transform.position);

                NormalizeViewPosInPlace(ref viewPos);

                Vector3 screenPos = new Vector3(screenBounds.width * viewPos.x, screenBounds.height * viewPos.y, 0f);

                //Is the point of interest behind the camera?
                if (viewPos.z < 0f)
                {
                    PositionToEdge(viewPos, screenBounds, ref screenPos);
                }

                UI.transform.localPosition = screenPos;
            } else
            {
                TrashUIs.Add(UI);
            }
        }

        if(TrashUIs.Count > 0)
        {
            foreach (PointOfInterestUI UI in TrashUIs)
            {
                UIs.Remove(UI);
                UI.gameObject.SetActive(false);
                Destroy(UI, 0.01f);
            }

            Debug.LogFormat("Trashed {0} points of interest", TrashUIs.Count);

            TrashUIs.Clear();
        }
    }

    /// <summary>
    /// Call me when adding or removing points of interest
    /// </summary>
    public void RefreshPOIList()
    {
        foreach (PointOfInterestUI UI in UIs)
        {
            GameObject.Destroy(UI.gameObject);
        }
        UIs.Clear();

        var POIs = GameObject.FindObjectsOfType<PointOfInterest>();

        foreach (var POI in POIs)
        {
            GameObject UIGO = GameObject.Instantiate(pointerUI);

            UIGO.transform.SetParent(transform);
            UIGO.transform.localScale = Vector3.one;
            UIGO.transform.localPosition = Vector3.zero;
            UIGO.transform.localRotation = Quaternion.Euler(Vector3.zero);

            PointOfInterestUI UI = UIGO.GetComponent<PointOfInterestUI>();

            if (UI != null)
            {
                UI.SetSubject(POI.gameObject);
                UIs.Add(UI);
            }
        }

        Debug.LogFormat("Refreshed {0} points of interest", UIs.Count);

        if (OnWaypointsChanged != null) 
            OnWaypointsChanged.Invoke();
        //pingSound.Play();
    }

    private void PositionToEdge(Vector3 viewPos, Rect screenBounds, ref Vector3 screenPos)
    {
        float nx, ny;
        if (viewPos.x < 0.5f)
        {
            nx = viewPos.x;
        }
        else
        {
            nx = 1f - viewPos.x;
        }

        if (viewPos.y < 0.5f)
        {
            ny = viewPos.y;
        }
        else
        {
            ny = 1f - viewPos.y;
        }

        if (ny <= nx)
        {
            if (viewPos.x < 0.5f)
            {
                screenPos.x = 0f;
            }
            else
            {
                screenPos.x = screenBounds.width = 0f;
            }
        }
        else
        {
            if (viewPos.y < 0.5f)
            {
                screenPos.y = 0f;
            }
            else
            {
                screenPos.y = screenBounds.height = 0f;
            }
        }
    }

    private void NormalizeViewPosInPlace(ref Vector3 ViewPos)
    {
        if (ViewPos.x < 0f) ViewPos.x = 0f;
        if (ViewPos.x > 1f) ViewPos.x = 1f;
        if (ViewPos.y < 0f) ViewPos.y = 0f;
        if (ViewPos.y > 1f) ViewPos.y = 1f;
    }

}
