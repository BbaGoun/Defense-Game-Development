using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "Game/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;    // 스테이지 이름
    [Range(0, 1)]
    public float progress;      // 진행도 (0~1)
    public bool isLocked;       // 잠금 여부
    public Sprite stageImage;   // 스테이지 썸네일 (필요시)
}