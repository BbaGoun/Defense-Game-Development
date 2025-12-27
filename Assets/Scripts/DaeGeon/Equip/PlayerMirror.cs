using System.Collections.Generic;
using UnityEngine;

public class PlayerMirror : MonoBehaviour
{
    public static PlayerMirror Instance;

    // 씬에 배치된 모든 Player 컴포넌트들
    private List<Player> registeredUnits = new List<Player>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Player 오브젝트들이 생성될 때 스스로를 등록함
    public void RegisterPlayer(Player unit)
    {
        if (!registeredUnits.Contains(unit))
            registeredUnits.Add(unit);
    }

    // 모든 유닛에게 장착 명령
    public void EquipAll(ItemData data)
    {
        foreach (var unit in registeredUnits)
            unit.Equip(data);
    }

    // 모든 유닛에게 미리보기 명령
    public void PreviewAll(ItemData data)
    {
        foreach (var unit in registeredUnits)
            unit.Preview(data);
    }

    // 모든 유닛 미리보기 초기화
    public void ClearAllPreviews()
    {
        foreach (var unit in registeredUnits)
            unit.ClearPreview();
    }
}