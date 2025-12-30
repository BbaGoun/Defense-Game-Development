using UnityEngine;

// 1. 장비 종류 정의
public enum EquipmentType 
{
    None,
    HELMET, 
    CHEST, 
    LEGS, 
    WEAPON, 
    BOOTS, 
    GLOVES
}

// 2. 외형 장착 위치 정의
public enum AttachPoint 
{
    None,
    HEAD, 
    FACE, 
    BODY
}

public enum ItemGrade 
{
    None,
    NORMAL,
    RARE,
    UNIQUE,
    LEGENDARY,
    MYTHIC
}

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("--- 공통 정보 ---")]
    public string itemID;      
    public string itemName;    
    public Sprite icon;        
    public int price;

    [TextArea(3, 5)] // 인스펙터에서 길게 쓸 수 있도록 추가
    public string description; // [추가] 상세 정보창 에러 해결 포인트

    [Header("--- 외형 설정 (외형 없으면 None) ---")]
    public GameObject equip;   // 실제 캐릭터에 붙는 모델링
    public GameObject prefab;  // 바닥에 떨어져 있을 때 등의 프리팹
    public AttachPoint attachPoint;

    [Header("--- 장비 설정 (장비 아니면 None) ---")]
    public EquipmentType equipmentType; 
    public int minAttack;
    public int maxAttack;
    public int minDefense;
    public int maxDefense;

    [Header("--- 장비 등급 ---")]
    public ItemGrade grade;

    [Header("--- 강화 설정 ---")]
    public ItemData upgradeMaterial; // 필요한 재료 데이터 (예: 강화석 SO)
    public int baseMaterialCount;    // 기본 필요 개수

    [Header("--- 분해 설정 ---")]
    public ItemData dismantleResult; // 분해 시 나올 재료 (강화석 등)
    public int dismantleAmount;     // 분해 시 획득할 개수
}