using UnityEngine;
using TMPro;

public class StageUIItem : MonoBehaviour
{
    public TextMeshProUGUI txtStageName;
    public TextMeshProUGUI txtProgress;

    public void SetData(StageData data)
    {
        txtStageName.text = data.stageName;
        int percent = Mathf.RoundToInt(data.progress * 100);
        txtProgress.text = $"진행도 : {percent:D2}%";
        
        // 잠금 로직이 필요하다면 여기서 data.isLocked를 활용해 처리 가능
    }
}