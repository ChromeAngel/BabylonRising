using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour {
    public List<MenuItem> items = new List<MenuItem>();
    private Vector2 MousePos;
    private Vector2 NormalisedMousePos = new Vector2(0.5f, 1f);
    private Vector2 centreCircle = new Vector2(0.5f, 0.5f);
    private Vector2 toVector2M;

    public GameObject MenuButtonPrefab;
    public GameObject MenuItemsContainer;

    public UnityEvent OnItemHighlighted;
    public UnityEvent<string> OnItemSelected;

    private List<RadialMenuButtonUI> MenuButtons;
    private float buttonArcDegrees;

    //private AudioSource TickSound;

    public static RadialMenu instance = null;

    private int _currentMenuItemIndex = 0;
    public int CurrentMenuItemIndex
    {
        get
        {
            return _currentMenuItemIndex;
        }
        set
        {
            if(MenuItemCount == 0)
            {
                value = -1;
            } else
            {
                while (value >= MenuItemCount) value = value - MenuItemCount;
                while (value < 0) value = value + MenuItemCount;
            }

            _currentMenuItemIndex = value;
        }
    }

    private int _menuItemCount;
    public int MenuItemCount
    {
        get
        {
            return _menuItemCount;
        }
    }

    private static bool is_open = true;

    public static bool IsOpen
    {
        get
        {
            return is_open;
        }
    }

    public static bool IsClosed
    {
        get
        {
            return !is_open;
        }
    }

    public void Open()
    {
        if (IsOpen) return;

        MenuItemsContainer.SetActive(true);
        is_open = true;
    }

    public void Close()
    {
        if (IsClosed) return;

        MenuItemsContainer.SetActive(false);
        is_open = false;
    }


    // Use this for initialization
    void Start () {
        centreCircle = new Vector2(0.5f,0.5f);
        _menuItemCount = items.Count;
        MenuButtons = new List<RadialMenuButtonUI>();
        //TickSound = GetComponent<AudioSource>();

        float buttonArc = 1f / _menuItemCount;
        buttonArcDegrees = buttonArc * 360f;
        int index = 0;

        foreach(MenuItem m in items)
        {
            GameObject menuButtonGo = GameObject.Instantiate(MenuButtonPrefab, MenuItemsContainer.transform);
            RadialMenuButtonUI menuButton = menuButtonGo.GetComponent<RadialMenuButtonUI>();
            menuButton.InitialiseButton(index, buttonArc, buttonArcDegrees, m, _menuItemCount);

            MenuButtons.Add(menuButton);

            index++;
        }

        Close();
    } //end Start

    public MenuItem CurrentMenuItem {
        get {
            return items[CurrentMenuItemIndex];
        }
    }

    public static float normalizeAngle(float source)
    {
        while (source > 360f) source = source - 360f;
        while (source < 0f) source = source + 360f;

        return source;
    }

    public void GetCurrentMenuItem()
    {
        MousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        NormalisedMousePos = new Vector2(MousePos.x / Screen.width, MousePos.y / Screen.height);
        float angleDegrees = Mathf.Atan2(NormalisedMousePos.y - centreCircle.y, NormalisedMousePos.x - centreCircle.x) * Mathf.Rad2Deg;

        angleDegrees = angleDegrees - (1.5f * buttonArcDegrees);  //fudging factor
        angleDegrees = normalizeAngle(angleDegrees);

        int menuItemIndex = Mathf.FloorToInt( angleDegrees / buttonArcDegrees );

        RadialMenuButtonUI menuButton = null;

        if (CurrentMenuItemIndex != menuItemIndex )
        {
            //Debug.LogFormat("menuItemIndex {0} from angle {1}", menuItemIndex, angleDegrees);

            if (CurrentMenuItemIndex > -1)
            {
                menuButton = MenuButtons[CurrentMenuItemIndex];
                menuButton.IsCurrent = false;
            }

            CurrentMenuItemIndex = menuItemIndex;

            menuButton = MenuButtons[CurrentMenuItemIndex];
            menuButton.IsCurrent = true;

            //TickSound.Play();
            if (OnItemHighlighted != null) OnItemHighlighted.Invoke();

            Debug.LogFormat("selected {0}", items[menuItemIndex].text);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if(IsOpen)
        {
            GetCurrentMenuItem();
            if(Input.GetButtonUp("Fire1"))
            {
                if(CurrentMenuItem.command != null )
                {
                   CurrentMenuItem.command(); 
                }

                if(CurrentMenuItem.OnClick != null )
                {
                    CurrentMenuItem.OnClick.Invoke();
                }

                if (OnItemSelected != null) OnItemSelected.Invoke(CurrentMenuItem.text);

                Close();
            }
        } else
        {
            if(Input.GetButton("Fire2"))
            {
                Open();
            }
        }
        
    }

    [System.Serializable]
    public class MenuItem
    {
        [TextArea()]
        public string text;
        public System.Action command;
        public bool enabled = true;
        public UnityEvent OnClick;
    }
}

