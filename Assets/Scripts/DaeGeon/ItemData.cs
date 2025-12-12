using UnityEngine;

public enum AttachPoint
{
    Head,
    Body
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;      
    public string itemName;    
    public Sprite icon;        
    public GameObject prefab;  
    public int price;          
    public AttachPoint attachPoint;
}
