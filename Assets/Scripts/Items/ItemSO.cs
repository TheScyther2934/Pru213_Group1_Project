using UnityEngine;
using System;

public enum ItemKind { TimedBuff, PermanentStat }

public abstract class ItemSO : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemKind kind;
    public bool stackable = true;   // cho phép dùng chồng?
}
