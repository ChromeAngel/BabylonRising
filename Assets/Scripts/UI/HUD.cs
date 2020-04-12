using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HUD : MonoBehaviour {
    public class colorPallete
    {
        public Color HUD;
        public Color Quest;
        public Color Enemy;
        public Color Ally;

        public Color pct80(Color baseColor)
        {
            return pct(baseColor, 80f);
        }

        public Color pct60(Color baseColor)
        {
            return pct(baseColor, 60f);
        }

        public Color pct40(Color baseColor)
        {
            return pct(baseColor,40f);
        }

        public Color pct20(Color baseColor)
        {
            return pct(baseColor, 20f);
        }

        private Color pct(Color baseColor, float percentage)
        {
            Color result = baseColor;

            result.a = percentage / 100f;

            return result;
        }

        private Color brighten(Color baseColor, float multiplier)
        {
            float h, s, v;
            Color.RGBToHSV(baseColor, out h, out s, out v);
            v = Mathf.Clamp( v * multiplier, 0f, 1f);
            Color result = Color.HSVToRGB(h, s, v);

            return result;
        }
    }

    private static void LoadColors()
    {
        var result = new colorPallete();

        result.HUD = GetColor("HUD_Color", "#62A4FF");
        result.Quest = GetColor("Quest_Color", "#FFB635");
        result.Enemy = GetColor("Enemy_Color", "#C63A23");
        result.Ally = GetColor("Ally_Color", "#B0FF45");

        _colors = result;
    }

    private static colorPallete _colors;

    public static colorPallete colors
    {
        get
        {
            if (_colors == null) LoadColors();

            return _colors;
        }
    }

    private static Color GetColor(string key, string defaultHex = "#FFFFFF")
    {
        Color result;
        string hex = defaultHex;

        if (PlayerPrefs.HasKey(key)) hex = PlayerPrefs.GetString(key);

        if(!ColorUtility.TryParseHtmlString(hex, out result))
        {
            ColorUtility.TryParseHtmlString(defaultHex, out result);
        }

        return result;
    }

    public Texture2D ClearCursor;
    private Combattent player_cmbt;

    void Start()
    {
        Cursor.SetCursor(ClearCursor, new Vector2(8f,8f), CursorMode.Auto);

        var _player = GameObject.FindGameObjectWithTag("Player");
        player_cmbt = _player.GetComponent<Combattent>();
    }

    public Color GetRelativeColor(GameObject subject)
    {
        Color DefaultResult = colors.HUD;

        //Is the subject part of an active Quest?
        var activeQuest = GameObject.FindObjectsOfType<Quest>().FirstOrDefault(x => x.status == Quest.Status.active && x.waypoint == subject);
        if(activeQuest != null)
        {
            DefaultResult = colors.Quest;
        }

        var target_cmbt = subject.GetComponent<Combattent>();

        //Relationship of player to target
        if (target_cmbt == null || player_cmbt == null)
        {
            return HUD.colors.pct60(DefaultResult);
        }

        var relation = player_cmbt.GetRelation(target_cmbt);

        switch (relation)
        {
            case Law.relation.Ally:
                return HUD.colors.Ally;
            case Law.relation.Competitor:
                return HUD.colors.pct60(HUD.colors.Enemy);
            case Law.relation.Hatred:
                return HUD.colors.Enemy;
            case Law.relation.Neutral:
                return HUD.colors.pct60(DefaultResult);
            case Law.relation.NonAggression:
                return HUD.colors.pct60(HUD.colors.Ally);
        }

        return HUD.colors.pct60(DefaultResult);
    }
}
