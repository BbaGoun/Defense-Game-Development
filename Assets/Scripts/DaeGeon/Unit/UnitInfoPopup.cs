using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DaeGeon
{
    public class UnitInfoPopup : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public Image gradeImg;

    public TMP_Text levelText;
    public TMP_Text shardText;
    public TMP_Text descText;

    [Header("Stats")]
    public TMP_Text attackText;
    public TMP_Text attackSpeedText;
    public TMP_Text rangeText;

    [Header("Buttons")]
    public Button upgradeButton;
    public TMP_Text upgradeButtonText;
    public Button closeButton;

    private UnitData data;
    private UnitState state;

    // =========================
    // 안전장치 (깜빡임 방지)
    // =========================
    private void Awake()
    {
        // 프리팹 실수 방지용 이중 안전장치
        if (upgradeButton != null)
            upgradeButton.interactable = false;
    }

    // =========================
    // 초기 세팅 (1회)
    // =========================
    public void Setup(UnitData data, UnitState state)
    {
        this.data = data;
        this.state = state;

        // 여기서도 절대 활성화 안 시킴
        upgradeButton.interactable = false;

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClick);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);

        Refresh();
    }

    // =========================
    // 상태 반영 전용
    // =========================
    void Refresh()
    {
        int need = data.shardsRequiredPerUpgrade;
        int current = state.shards;

        // 기본 정보
        icon.sprite = data.icon;
        nameText.text = data.unitName;
        gradeImg.sprite = UnitManager.Instance.GetGradeSprite(data.grade);

        descText.text = data.description;
        shardText.text = $"{current} / {need}";
        levelText.text = state.owned ? $"Lv.{state.level}" : "미보유";

        // 스탯
        attackText.text = $"공격력 : {data.baseAttack}";
        attackSpeedText.text = $"공격속도 : {data.baseAttackSpeed}";
        rangeText.text = $"사거리 : {data.baseRange}";

        // 🔑 강화 조건
        bool canUpgrade = state.owned && current >= need;

        // 조건 만족 시에만 활성화
        upgradeButton.interactable = canUpgrade;

        upgradeButtonText.text = need.ToString();
    }

    // =========================
    // 강화 로직
    // =========================
    void OnUpgradeClick()
    {
        if (!upgradeButton.interactable)
            return;

        bool success = UnitManager.Instance.TryUpgrade(data.unitId);
        if (!success)
            return;

        state = UnitManager.Instance.GetState(data.unitId);

        // 강화 후에는 일단 비활성화
        upgradeButton.interactable = false;

        Refresh();

        if (UnitUI.Instance != null)
            UnitUI.Instance.RefreshUI();
    }

    void Close()
    {
        Destroy(gameObject);
    }
}
}
