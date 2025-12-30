using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Sangmin
{
    /// <summary>
    /// Cell 선택 시 화면에 표시되는 Cell 정보 패널 (활성화/비활성화 제어)
    /// </summary>
    public class CellInfoPanel : MonoBehaviour
    {
        private static CellInfoPanel _instance;
        public static CellInfoPanel Instance
        {
            get { return _instance; }
        }

        [Header("UI References")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI cellPositionText;
        [SerializeField] private TextMeshProUGUI cellStatusText;
        [SerializeField] private Button activateButton;
        [SerializeField] private Button deactivateButton;

        private UnitCell currentSelectedCell;

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
            if (activateButton != null)
                activateButton.onClick.AddListener(OnActivateButtonClicked);

            if (deactivateButton != null)
                deactivateButton.onClick.AddListener(OnDeactivateButtonClicked);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        /// Cell 정보를 표시
        /// </summary>
        public void ShowCellInfo(UnitCell cell)
        {
            if (cell == null)
            {
                HideCellInfo();
                return;
            }

            currentSelectedCell = cell;

            if (panelRoot != null)
                panelRoot.SetActive(true);

            UpdateCellInfoDisplay(cell);
        }

        /// <summary>
        /// Cell 정보 패널을 숨김
        /// </summary>
        public void HideCellInfo()
        {
            currentSelectedCell = null;

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        /// <summary>
        /// Cell 정보를 UI에 업데이트
        /// </summary>
        private void UpdateCellInfoDisplay(UnitCell cell)
        {
            if (cell == null)
                return;

            // Cell 위치 정보
            if (cellPositionText != null)
                cellPositionText.text = $"위치: ({cell.row}, {cell.col})";

            // Cell 상태 정보
            if (cellStatusText != null)
            {
                string status = cell.isCellActive ? "활성화" : "비활성화";
                string occupied = cell.isOccupied ? " (유닛 배치됨)" : " (빈 셀)";
                cellStatusText.text = $"상태: {status}{occupied}";
            }

            // 버튼 활성화/비활성화 상태 업데이트
            if (activateButton != null)
            {
                // 활성화 버튼은 비활성화된 셀일 때만 활성화
                activateButton.interactable = !cell.isCellActive;
            }

            if (deactivateButton != null)
            {
                // 비활성화 버튼은 활성화된 셀이고 유닛이 없을 때만 활성화
                deactivateButton.interactable = cell.isCellActive && !cell.isOccupied;
            }
        }

        /// <summary>
        /// 활성화 버튼 클릭 시 호출
        /// </summary>
        private void OnActivateButtonClicked()
        {
            if (currentSelectedCell != null && GridUnitPlacement.Instance != null)
            {
                bool success = GridUnitPlacement.Instance.SetCellActive(currentSelectedCell.gameObject, true);
                if (success)
                {
                    // 하이라이트 갱신
                    GridUnitPlacement.Instance.DrawHighlight();

                    // 네비게이션 재생성
                    NavMesh2D.Instance.RebuildNavigation();

                    // UI 업데이트
                    UpdateCellInfoDisplay(currentSelectedCell);
                }
            }
        }

        /// <summary>
        /// 비활성화 버튼 클릭 시 호출
        /// </summary>
        private void OnDeactivateButtonClicked()
        {
            if (currentSelectedCell != null && GridUnitPlacement.Instance != null)
            {
                bool success = GridUnitPlacement.Instance.SetCellActive(currentSelectedCell.gameObject, false);
                if (success)
                {
                    // 하이라이트 갱신
                    GridUnitPlacement.Instance.DrawHighlight();

                    // 네비게이션 재생성
                    NavMesh2D.Instance.RebuildNavigation();

                    // UI 업데이트
                    UpdateCellInfoDisplay(currentSelectedCell);
                }
            }
        }
    }
}

