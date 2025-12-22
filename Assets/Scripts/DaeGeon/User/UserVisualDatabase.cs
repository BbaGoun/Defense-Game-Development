using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "User/UserVisualDB")]
public class UserVisualDatabase : ScriptableObject
{
    public List<UserIconEntry> icons;
    public List<UserFrameEntry> frames;

    public UserIconEntry GetIconEntry(string id)
        => icons.Find(x => x.id == id);

    public UserFrameEntry GetFrameEntry(string id)
        => frames.Find(x => x.id == id);
}

[System.Serializable]
public class UserIconEntry
{
    public string id;
    public Sprite sprite;

    [Header("Meta")]
    public string displayName;
    [TextArea] public string acquireDesc;
}

[System.Serializable]
public class UserFrameEntry
{
    public string id;
    public Sprite sprite;

    [Header("Meta")]
    public string displayName;
    [TextArea] public string acquireDesc;
}
