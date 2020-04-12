using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NebulaCloud : MonoBehaviour {
    public float radius;

    private SpriteRenderer sprite;
    private Camera mainCamera;
    private bool CloseLastFrame = false;
    private List<SpriteRenderer> children;
    private float initalAlpha;
    // Use this for initialization
    void Start () {
        sprite = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        children = new List<SpriteRenderer>();
        initalAlpha = sprite.color.a;
    }
	
	// Update is called once per frame
	void Update () {
        float proximity = Vector3.Distance(transform.position, mainCamera.transform.position);

        bool IsInRangeAndLookingAtMe = IsLookingAtMe() && (proximity <= radius);

        transform.LookAt(mainCamera.transform);

        if (CloseLastFrame)
        {
            if(IsInRangeAndLookingAtMe)
            {
                //still active
                float ratio = proximity / radius; //1 at the edge 0 at the center
                Color col = sprite.color;
                Color childColor = sprite.color;
                col.a = initalAlpha * ratio;
                childColor.a = initalAlpha * (1f - ratio);
                sprite.color = col;
                sprite.enabled = true;

                foreach (var childSprite in children)
                {
                    var goChild = childSprite.gameObject;

                    childSprite.color = childColor;
                }
            } else
            {
                //become inactive
                sprite.enabled = false;

                //foreach (var childSprite in children)
                //{
                //    childSprite.enabled = false;
                //}
            }
        } else
        {
            if (IsInRangeAndLookingAtMe)
            {
                //become active
                float ratio = proximity / radius; //1 at the edge 0 at the center
                Color col = sprite.color;
                Color childColor = sprite.color;
                col.a = initalAlpha * ratio;
                childColor.a = initalAlpha * (1f - ratio);
                sprite.color = col;
                sprite.enabled = true;

                if (children.Count == 0)
                {
                    //TODO Create children

                    for (int i = 0; i < 6; i++)
                    {
                        var goChild = new GameObject(name + "_" + i.ToString());
                        goChild.transform.SetParent(transform);
                        goChild.transform.position = Chance.VectorOnSphere(Vector3.zero, radius / 2f);
                        goChild.transform.localScale = transform.localScale * 0.5f;
                        goChild.transform.rotation = Quaternion.Euler(0f, 0f, Chance.RandomFloat(359f));

                        var childSprite = goChild.AddComponent<SpriteRenderer>();
                        childSprite.sprite = sprite.sprite;
                        childSprite.color = childColor;

                        var cloud = goChild.AddComponent<NebulaCloud>();
                        cloud.radius = radius / 2f;

                        children.Add(childSprite);
                    }
                } else
                {
                    //foreach (var childSprite in children)
                    //{
                    //    childSprite.color = childColor;
                    //    childSprite.enabled = true;
                    //}
                }
            }
            else
            {
                //remain idle
            }
        }

        CloseLastFrame = IsInRangeAndLookingAtMe;
    }

    private bool IsLookingAtMe()
    {
        return true;

        //Vector3 cameraforward = mainCamera.transform.TransformDirection(Vector3.forward);
        //var vectorToMe = (transform.position - mainCamera.transform.position);

        //return (Vector3.Dot(cameraforward, vectorToMe) > 0);
    }
}
