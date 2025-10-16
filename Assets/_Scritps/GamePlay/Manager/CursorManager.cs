using System.Collections.Generic;
using UnityEngine;

public class CursorManager
{
    private static readonly Dictionary<string, Texture2D> cursorMap = new();

    public static void Init()
    {
        CursorsSO cursorSO = Resources.Load<CursorsSO>("MyCursor");

        foreach (MyCursor myCursor in cursorSO.cursors)
        {
            cursorMap.TryAdd(myCursor.key, myCursor.icon);
        }

        Cursor.SetCursor(cursorMap["default"], Vector2.zero, CursorMode.Auto);
    }

    public static void DefaultCursorIcon()
    {
        Cursor.SetCursor(cursorMap["default"], Vector2.zero, CursorMode.Auto);
    }

    public static void ChangeCursorIcon(string key, bool center = false)
    {
        Texture2D tex = cursorMap[key];
        Vector2 hotSpot = Vector2.zero;

        if (center)
            hotSpot = new Vector2(tex.width / 2f, tex.height / 2f);

        Cursor.SetCursor(tex, hotSpot, CursorMode.Auto);
    }

    public static Texture2D GetCursorIcon(string key)
    {
        return cursorMap[key];
    }
}
