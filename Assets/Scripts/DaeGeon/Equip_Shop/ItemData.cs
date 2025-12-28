using UnityEngine;

// 1. 장비 종류 정의
public enum EquipmentType 
{
    None,   // 외형 전용 아이템일 경우
    Helmet, 
    Chest, 
    Legs, 
    Weapon, 
    Boots, 
    Gloves
}

// 2. 외형 장착 위치 정의
public enum AttachPoint 
{
    None,   // 장비 전용 아이템일 경우
    Head, 
    Face, 
    Body
}

public enum ItemGrade 
{
    None,
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("--- 공통 정보 ---")]
    public string itemID;      
    public string itemName;    
    public Sprite icon;        
    public int price;
    public bool isEquipped;

    [Header("--- 외형 설정 (외형 없으면 None) ---")]
    public GameObject equip;   // 실제 캐릭터에 붙는 모델링
    public GameObject prefab;  // 관련 구조물
    public AttachPoint attachPoint;

    [Header("--- 장비 설정 (장비 아니면 None) ---")]
    public EquipmentType equipmentType; 
    public int minAttack;
    public int maxAttack;
    public int minDefense;
    public int maxDefense;

    [Header("--- 장비 등급 ---")]
    public ItemGrade grade;
}