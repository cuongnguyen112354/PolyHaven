using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCursors", menuName = "ScriptableObjects/Cursors")]
public class CursorsSO : ScriptableObject
{
    public List<MyCursor> cursors;
}

[Serializable]
public class MyCursor
{
    public string key;
    public Texture2D icon;
}