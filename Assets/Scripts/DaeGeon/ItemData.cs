using UnityEngine;

public enum AttachPoint
{
    Head,
    Face,
    Body
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;      
    public string itemName;    
    public Sprite icon;        
    public GameObject equip; //실제 장착시 보여지는 스케일/위치 등 이미지
    public bool isEquipped;  //장착 여부
    public GameObject prefab;  //해당 구조
    public int price;          
    public AttachPoint attachPoint;
}
