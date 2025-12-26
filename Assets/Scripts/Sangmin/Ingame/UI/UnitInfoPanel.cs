using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Sangmin
{
    /// <summary>
    /// 유닛 선택 시 화면 우측에 표시되는 유닛 정보 패널
    /// </summary>
    public class UnitInfoPanel : MonoBehaviour
    {
        private static UnitInfoPanel _instance;
        public static UnitInfoPanel Instance
        {
            get { return _instance; }
        }

        [Header("UI References")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private TextMeshProUGUI attackDamageText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI attackRangeText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private Button sellButton;
        [SerializeField] private Button rotateChainButton;

        private Unit currentSelectedUnit;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);

            // 패널 초기에는 비활성화
            if (panelRoot != null)
                panelRoot.SetActive(false);

            // 버튼 이벤트 연결
            if (sellButton != null)
                sellButton.onClick.AddListener(OnSellButtonClicked);

            if (rotateChainButton != null)
                rotateChainButton.onClick.AddListener(OnRotateChainButtonClicked);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        /// 유닛 정보를 표시
        /// </summary>
        public void ShowUnitInfo(Unit unit)
        {
            if (unit == null)
            {
                HideUnitInfo();
                return;
            }

            currentSelectedUnit = unit;
            
            if (panelRoot != null)
                panelRoot.SetActive(true);

            UpdateUnitInfoDisplay(unit);
        }

        /// <summary>
        /// 유닛 정보 패널을 숨김
        /// </summary>
        public void HideUnitInfo()
        {
            currentSelectedUnit = null;
            
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        /// <summary>
        /// 유닛 정보를 UI에 업데이트
        /// </summary>
        private void UpdateUnitInfoDisplay(Unit unit)
        {
            if (unit == null)
                return;

            // 유닛 이름
            if (unitNameText != null)
                unitNameText.text = unit.gameObject.name;

            // 공격력
            if (attackDamageText != null){
                float diffAtkDmg = unit.finalAttackDamage-unit.unitStatData.attackDamage;
                char oper = diffAtkDmg >= 0 ? '+' : '-';
                attackDamageText.text = $"공격력: {unit.unitStatData.attackDamage}{oper}{diffAtkDmg:F1}";
            
            }
            // 공격 속도
            if (attackSpeedText != null){
                float diffAtkSpd = unit.finalAttackSpeed-unit.unitStatData.attackSpeed;
                char oper = diffAtkSpd >= 0 ? '+' : '-';
                attackSpeedText.text = $"공격 속도: {unit.unitStatData.attackSpeed}{oper}{diffAtkSpd:F2}";
            }

            // 사거리
            if (attackRangeText != null){
                int diffAtkRange = unit.finalAttackRange-unit.unitStatData.attackRange;
                char oper = diffAtkRange >= 0 ? '+' : '-';
                attackRangeText.text = $"사거리: {unit.unitStatData.attackRange}{oper}{diffAtkRange}";
            }

            // 등급
            if (gradeText != null)
            {
                string gradeName = GetGradeName(unit.unitStatData.grade);
                gradeText.text = $"등급: {gradeName}";
            }
        }

        /// <summary>
        /// 등급 이름을 한국어로 반환
        /// </summary>
        private string GetGradeName(UnitStatData.Grade grade)
        {
            switch (grade)
            {
                case UnitStatData.Grade.NORMAL:
                    return "일반";
                case UnitStatData.Grade.RARE:
                    return "희귀";
                case UnitStatData.Grade.HERO:
                    return "영웅";
                case UnitStatData.Grade.LEGEND:
                    return "전설";
                case UnitStatData.Grade.MYTHIC:
                    return "신화";
                default:
                    return "알 수 없음";
            }
        }

        /// <summary>
        /// 판매 버튼 클릭 시 호출
        /// </summary>
        private void OnSellButtonClicked()
        {
            if (GridUnitPlacement.Instance != null)
            {
                GridUnitPlacement.Instance.SellUnit();
                HideUnitInfo();
            }
        }

        /// <summary>
        /// 체인 방향 회전 버튼 클릭 시 호출
        /// </summary>
        private void OnRotateChainButtonClicked()
        {
            if (currentSelectedUnit != null && GridUnitPlacement.Instance != null)
            {
                UnitCell selectedCell = GridUnitPlacement.Instance.GetSelectedCell();
                if (selectedCell != null)
                {
                    Vector2Int gridPos = new Vector2Int(selectedCell.row, selectedCell.col);
                    Unit.ChainDirection oldChain = currentSelectedUnit.chain;
                    
                    // 체인 회전
                    currentSelectedUnit.RotateChainClockwise();
                    
                    // 시너지 시스템에 체인 변경 알림
                    if (SynergyCountSystem.Instance != null)
                    {
                        // 체인 변경 후 시너지 시스템 업데이트
                        SynergyCountSystem.Instance.UpdateUnitChain(gridPos, currentSelectedUnit.chain);
                    }
                }
            }
        }
    }
}

