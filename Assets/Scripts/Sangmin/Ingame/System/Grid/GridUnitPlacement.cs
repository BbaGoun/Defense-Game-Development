using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System;

namespace Sangmin
{
    public class GridUnitPlacement : MonoBehaviour
    {
        private static GridUnitPlacement _instance;
        public static GridUnitPlacement Instance
        {
            get { return _instance; }
        }

        [Header("Grid Option")]
        // 행(세로, row) 개수
        public int gridHeight = 4;
        // 열(가로, column) 개수
        public int gridWidth = 6;
        public GameObject gridRoot;
        public float cellSize = 1.0f;

        private int unitCount;

        [Header("Unit Limit")]
        [Tooltip("소환할 수 있는 최대 유닛 수")]
        [SerializeField] private int unitCountMax = 21;
        private int previousUnitCountMax; // 이전 값 추적용

        public GameObject gridParent;

        // 유닛 수 변경 이벤트
        public Action<int, int> OnUnitCountChanged; // (현재 유닛 수, 최대 유닛 수)

        public bool isCellSelected => selectedCell != null;
        public UnitCell[,] cellInfos { get; private set; }
        [SerializeField]
        private UnitCell selectedCell;
        private Unit currentSelectedUnit;

        [Header("Colors")]
        [SerializeField] private LineRenderer dragLine;
        public Color selectedColor = Color.yellow;
        public Color availableColor = Color.green;
        public Color blockedColor = Color.red;
        public Color dragPathColor = Color.cyan;
        public Color dragTargetColor = Color.blue;
        private UnitCell dragTargetCell;

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        // Public Properties
        public int UnitCount => unitCount;
        public int UnitCountMax => unitCountMax;
        public bool IsUnitLimitReached => unitCount >= unitCountMax;

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);

            cellSize = gridRoot.transform.localScale.x;

            unitCount = 0;
            previousUnitCountMax = unitCountMax;

            // [행, 열] 순서로 2차원 배열 생성
            cellInfos = new UnitCell[gridHeight, gridWidth];

            // 1차원 배열(unitCells)을 (행, 열) 순서의 2차원 배열로 변경
            UnitCell[] unitCells = gridParent.GetComponentsInChildren<UnitCell>();
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    int index = row * gridWidth + col; // 행 우선(row-major) 인덱싱
                    if (index < unitCells.Length)
                    {
                        cellInfos[row, col] = unitCells[index];
                        // Debug.Log($"cellInfos[{row}, {col}] 대입 : {unitCells[index].name}");
                    }
                }
            }

            EnsureDragLine();
        }

        void Start()
        {

        }

        [Header("유닛 구매/판매 설정")]
        [SerializeField] private int unitSellPrice = 1; // 유닛 판매 가격

        /// <summary>
        /// 일반 뽑기 (골드 사용)
        /// </summary>
        public void PlaceUnitFromFront()
        {
            if (unitCount >= unitCountMax)
            {
                Debug.LogWarning($"유닛 소환 실패: 유닛 수 한계 도달 (현재: {unitCount}/{unitCountMax})");
                return;
            }

            // 뽑기 비용 확인 및 소비 (뽑기 비용은 IngameCurrencyManager에서 관리)
            if (IngameCurrencyManager.Instance == null || !IngameCurrencyManager.Instance.SpendSummonCost())
            {
                int currentCost = IngameCurrencyManager.Instance != null ? IngameCurrencyManager.Instance.CurrentSummonCost : 0;
                Debug.LogWarning($"유닛 구매 실패: 골드 부족 (필요: {currentCost})");
                return;
            }

            var unit = RandomSummon.Instance.SummonRandomUnit();
            if (unit == null)
            {
                Debug.LogError("유닛 뽑기 실패!");
                IngameCurrencyManager.Instance?.AddGold(IngameCurrencyManager.Instance.CurrentSummonCost); // 골드 환불
                return;
            }

            // 유닛 배치 시도
            if (PlaceUnitOnGrid(unit))
            {
                // 배치 성공 시에만 카운트 증가
                unitCount++;
                OnUnitCountChanged?.Invoke(unitCount, unitCountMax);
            }
            else
            {
                // 배치 실패 시 골드 환불
                IngameCurrencyManager.Instance?.AddGold(IngameCurrencyManager.Instance.CurrentSummonCost);
            }
        }

        /// <summary>
        /// 희귀 등급 뽑기 (쥬얼 사용)
        /// </summary>
        public void PlaceRareUnit()
        {
            if (unitCount >= unitCountMax)
            {
                Debug.LogWarning($"유닛 소환 실패: 유닛 수 한계 도달 (현재: {unitCount}/{unitCountMax})");
                return;
            }

            // 쥬얼 확인 및 소비
            if (IngameCurrencyManager.Instance == null || !IngameCurrencyManager.Instance.SpendRareSummonCost())
            {
                int cost = IngameCurrencyManager.Instance != null ? IngameCurrencyManager.Instance.RareSummonCost : 0;
                Debug.LogWarning($"희귀 등급 뽑기 실패: 쥬얼 부족 (필요: {cost})");
                return;
            }

            var unit = RandomSummon.Instance != null ? RandomSummon.Instance.SummonRareUnit() : null;
            if (unit == null)
            {
                Debug.LogError("희귀 등급 유닛 뽑기 실패!");
                IngameCurrencyManager.Instance?.AddJewel(IngameCurrencyManager.Instance.RareSummonCost); // 쥬얼 환불
                return;
            }

            // 유닛 배치 시도
            if (PlaceUnitOnGrid(unit))
            {
                // 배치 성공 시에만 카운트 증가
                unitCount++;
                OnUnitCountChanged?.Invoke(unitCount, unitCountMax);
            }
            else
            {
                // 배치 실패 시 쥬얼 환불
                IngameCurrencyManager.Instance?.AddJewel(IngameCurrencyManager.Instance.RareSummonCost);
            }
        }

        /// <summary>
        /// 영웅 등급 뽑기 (쥬얼 사용)
        /// </summary>
        public void PlaceHeroUnit()
        {
            if (unitCount >= unitCountMax)
            {
                Debug.LogWarning($"유닛 소환 실패: 유닛 수 한계 도달 (현재: {unitCount}/{unitCountMax})");
                return;
            }

            // 쥬얼 확인 및 소비
            if (IngameCurrencyManager.Instance == null || !IngameCurrencyManager.Instance.SpendHeroSummonCost())
            {
                int cost = IngameCurrencyManager.Instance != null ? IngameCurrencyManager.Instance.HeroSummonCost : 0;
                Debug.LogWarning($"영웅 등급 뽑기 실패: 쥬얼 부족 (필요: {cost})");
                return;
            }

            var unit = RandomSummon.Instance != null ? RandomSummon.Instance.SummonHeroUnit() : null;
            if (unit == null)
            {
                Debug.LogError("영웅 등급 유닛 뽑기 실패!");
                IngameCurrencyManager.Instance?.AddJewel(IngameCurrencyManager.Instance.HeroSummonCost); // 쥬얼 환불
                return;
            }

            // 유닛 배치 시도
            if (PlaceUnitOnGrid(unit))
            {
                // 배치 성공 시에만 카운트 증가
                unitCount++;
                OnUnitCountChanged?.Invoke(unitCount, unitCountMax);
            }
            else
            {
                // 배치 실패 시 쥬얼 환불
                IngameCurrencyManager.Instance?.AddJewel(IngameCurrencyManager.Instance.HeroSummonCost);
            }
        }

        /// <summary>
        /// 전설 등급 뽑기 (쥬얼 사용)
        /// </summary>
        public void PlaceLegendUnit()
        {
            if (unitCount >= unitCountMax)
            {
                Debug.LogWarning($"유닛 소환 실패: 유닛 수 한계 도달 (현재: {unitCount}/{unitCountMax})");
                return;
            }

            // 쥬얼 확인 및 소비
            if (IngameCurrencyManager.Instance == null || !IngameCurrencyManager.Instance.SpendLegendSummonCost())
            {
                int cost = IngameCurrencyManager.Instance != null ? IngameCurrencyManager.Instance.LegendSummonCost : 0;
                Debug.LogWarning($"전설 등급 뽑기 실패: 쥬얼 부족 (필요: {cost})");
                return;
            }

            var unit = RandomSummon.Instance != null ? RandomSummon.Instance.SummonLegendUnit() : null;
            if (unit == null)
            {
                Debug.LogError("전설 등급 유닛 뽑기 실패!");
                IngameCurrencyManager.Instance?.AddJewel(IngameCurrencyManager.Instance.LegendSummonCost); // 쥬얼 환불
                return;
            }

            // 유닛 배치 시도
            if (PlaceUnitOnGrid(unit))
            {
                // 배치 성공 시에만 카운트 증가
                unitCount++;
                OnUnitCountChanged?.Invoke(unitCount, unitCountMax);
            }
            else
            {
                // 배치 실패 시 쥬얼 환불
                IngameCurrencyManager.Instance?.AddJewel(IngameCurrencyManager.Instance.LegendSummonCost);
            }
        }

        /// <summary>
        /// 유닛을 그리드에 배치하는 공통 로직
        /// </summary>
        /// <returns>배치 성공 여부</returns>
        private bool PlaceUnitOnGrid(Unit unit)
        {
            if (unit == null) return false;

            // 행(row)을 먼저, 그 다음 열(col)을 순회
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    // 비활성화된 셀이거나 이미 점유된 셀은 건너뛰기
                    if (cellInfos[row, col] == null || !cellInfos[row, col].isCellActive || cellInfos[row, col].isOccupied)
                        continue;

                    // 시너지 계산 시스템에 Unit을 생성하는 코드
                    SynergyCountSystem.Instance.SpawnUnit(new Vector2Int(row, col), mask: unit.chain, unit);
                    // UnitCell에 유닛을 배정하는 코드
                    cellInfos[row, col].PlaceUnit(unit);

                    if (selectedCell != null && currentSelectedUnit != null)
                    {
                        SynergyCountSystem.Instance.OutlineConnectedNode(new Vector2Int(selectedCell.row, selectedCell.col));
                    }
                    return true;
                }
            }

            // 배치할 수 있는 셀이 없음
            Debug.LogWarning("유닛 배치 실패: 사용 가능한 셀이 없습니다.");
            return false;
        }

        public void SellUnit()
        {
            if (selectedCell == null)
                return;

            Unit unitToSell = selectedCell.GetUnit();
            if (unitToSell == null)
                return;

            Debug.Log($"Sell Unit: {unitToSell.name}");
            unitCount--;
            OnUnitCountChanged?.Invoke(unitCount, unitCountMax);

            // 골드 추가
            if (IngameCurrencyManager.Instance != null)
            {
                IngameCurrencyManager.Instance.AddGold(unitSellPrice);
            }

            // 셀에서 유닛 제거
            unitToSell.OnSell();

            SynergyCountSystem.Instance.SellUnit(new Vector2Int(selectedCell.row, selectedCell.col));

            selectedCell.ClearUnit();

            // 선택 해제
            UnSelectUnit();
        }

        public bool SelectCell(GameObject cell)
        {
            //Debug.Log($"Selected cell: {cell.name}");

            // 뭐가 이미 선택되어 있을 지 모르니까 초기화
            UnSelectUnit();

            // (행, 열) 순서로 탐색
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cellInfos[row, col] == null)
                        continue;

                    if (cell.Equals(cellInfos[row, col].gameObject))
                    {
                        // 셀 안에 유닛 있는지 확인, 확인이 되면 유닛이 있는 유의미한 셀을 선택한 것
                        if (cellInfos[row, col].isOccupied)
                        {
                            selectedCell = cellInfos[row, col];

                            // 시간 느려지기
                            TimeController.Instance.SetTimeScale(0.2f);

                            // 유닛이 이미 놓여져 있는지 색깔로 여부 표시
                            DrawHighlight();

                            // 유닛 선택 시 사거리 표시 및 UI 패널 표시
                            Unit selectedUnit = selectedCell.GetUnit();
                            if (selectedUnit != null)
                            {
                                SelectUnit(selectedUnit);
                            }

                            SynergyCountSystem.Instance.OutlineConnectedNode(new Vector2Int(row, col));

                            return false;
                        }
                        else
                        {
                            // Debug.Log($"유닛이 없는 셀을 선택한 경우 CellInfoPanel 표시: {cellInfos[row, col].name}");

                            // 유닛이 없는 셀을 선택한 경우 CellInfoPanel 표시
                            selectedCell = cellInfos[row, col];

                            // 시간 느려지기
                            TimeController.Instance.SetTimeScale(0.2f);

                            // 하이라이트 표시
                            DrawHighlight();

                            // CellInfoPanel 표시
                            if (CellInfoPanel.Instance != null)
                            {
                                CellInfoPanel.Instance.ShowCellInfo(selectedCell);
                            }

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public UnitCell GetSelectedCell()
        {
            return selectedCell;
        }

        public void UnSelectUnit()
        {
            //Debug.Log("UnSelect");

            if (selectedCell == null)
                return;

            // 이전 선택된 유닛의 사거리 표시 숨기기
            if (currentSelectedUnit != null)
            {
                currentSelectedUnit.HideRange();
            }
            currentSelectedUnit = null;

            // UI 패널 숨기기
            if (UnitInfoPanel.Instance != null)
            {
                UnitInfoPanel.Instance.HideUnitInfo();
            }

            // CellInfoPanel 숨기기
            if (CellInfoPanel.Instance != null)
            {
                CellInfoPanel.Instance.HideCellInfo();
            }

            SynergyCountSystem.Instance.OutlineClear();

            selectedCell = null;

            // 시간 정상화
            TimeController.Instance.SetTimeScale(1f);

            ClearHighlight();
        }

        /// <summary>
        /// 유닛 선택 시 사거리 표시 및 UI 패널 표시
        /// </summary>
        private void SelectUnit(Unit unit)
        {
            // 이전 선택된 유닛의 사거리 표시 숨기기
            if (currentSelectedUnit != null && currentSelectedUnit != unit)
            {
                currentSelectedUnit.HideRange();
            }

            currentSelectedUnit = unit;

            // 사거리 표시
            if (unit != null)
            {
                unit.ShowRange();
            }

            // UI 패널 표시
            if (UnitInfoPanel.Instance != null)
            {
                UnitInfoPanel.Instance.ShowUnitInfo(unit);
            }
        }

        /// <summary>
        /// 선택된 셀의 유닛을 주어진 셀로 이동시킨다.
        /// 대상 셀에 이미 유닛이 있으면 두 유닛의 위치를 교환한다.
        /// </summary>
        public void MoveUnit(GameObject cell)
        {
            if (selectedCell == null || cell == null)
                return;

            UnitCell targetCell = FindCellByGameObject(cell);
            if (targetCell == null || !targetCell.isCellActive)
                return;

            Unit movingUnit = selectedCell.GetUnit();
            if (movingUnit == null)
                return;

            // 이미 유닛이 있는 칸일 경우 위치 교환
            if (targetCell.isOccupied)
            {
                Unit targetUnit = targetCell.GetUnit();
                if (targetUnit == null)
                    return;

                // 두 유닛의 위치 교환
                SynergyCountSystem.Instance.SwapUnit(new Vector2Int(selectedCell.row, selectedCell.col), new Vector2Int(targetCell.row, targetCell.col));

                selectedCell.ClearUnit();
                targetCell.ClearUnit();

                targetCell.PlaceUnit(movingUnit);
                selectedCell.PlaceUnit(targetUnit);

                // 선택 대상 셀을 새 위치로 갱신
                selectedCell = targetCell;
            }
            else
            {
                SynergyCountSystem.Instance.MoveUnit(new Vector2Int(selectedCell.row, selectedCell.col), new Vector2Int(targetCell.row, targetCell.col));

                selectedCell.ClearUnit();
                targetCell.PlaceUnit(movingUnit);

                // 선택 대상 셀을 새 위치로 갱신
                selectedCell = targetCell;
            }

            SynergyCountSystem.Instance.OutlineConnectedNode(new Vector2Int(targetCell.row, targetCell.col));

            // 하이라이트 갱신
            ClearHighlight();
            DrawHighlight();
        }

        /// <summary>
        /// 드래그로 유닛을 이동할 때의 시작 처리
        /// </summary>
        public void BeginDrag()
        {
            if (selectedCell == null)
                return;

            if (selectedCell.GetUnit() == null)
                return;

            if (dragLine == null)
                EnsureDragLine();

            dragLine.enabled = true;
            dragTargetCell = null;
        }

        /// <summary>
        /// 드래그 중에 호출되어 경로(선)와 도착 셀 하이라이트를 갱신한다.
        /// </summary>
        public void UpdateDrag(Vector3 mouseWorldPos, GameObject hoverCellObject)
        {
            if (selectedCell == null || dragLine == null)
                return;

            if (selectedCell.GetUnit() == null)
                return;

            // 점선/경로: 선택된 셀 중심에서 마우스 위치까지
            Vector3 start = selectedCell.transform.position;
            Vector3 end = mouseWorldPos;
            end.z = start.z;

            dragLine.SetPosition(0, start);
            dragLine.SetPosition(1, end);

            // 현재 마우스가 올라가 있는 셀 찾기
            UnitCell newTargetCell = FindCellByGameObject(hoverCellObject);

            // 이전 타겟 셀의 색을 원래대로 되돌림
            if (dragTargetCell != null && dragTargetCell != selectedCell)
            {
                if (dragTargetCell.isOccupied)
                    dragTargetCell.SetHighlight(true, blockedColor);
                else
                    dragTargetCell.SetHighlight(true, availableColor);
            }

            if (newTargetCell == null)
            {
                dragTargetCell = null;
                return;
            }

            if (!newTargetCell.isCellActive)
                dragTargetCell = null;
            else
                dragTargetCell = newTargetCell;

            // 새 타겟 셀을 드래그 목적지 색으로 표시 (자기 자신은 제외)
            if (dragTargetCell != null && dragTargetCell != selectedCell)
            {
                dragTargetCell.SetHighlight(true, dragTargetColor);
            }
        }

        /// <summary>
        /// 드래그 종료 시 호출. 도착 위치로 MoveUnit을 실행한다.
        /// </summary>
        public void EndDrag()
        {
            if (dragLine != null)
            {
                dragLine.enabled = false;
            }

            if (dragTargetCell != null && dragTargetCell != selectedCell)
            {
                MoveUnit(dragTargetCell.gameObject);
            }

            dragTargetCell = null;
        }

        public void DrawHighlight()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cellInfos[row, col] == null)
                        continue;

                    if (cellInfos[row, col].Equals(selectedCell))
                        cellInfos[row, col].SetHighlight(true, selectedColor);
                    else if (cellInfos[row, col].isCellActive)
                    {
                        if (cellInfos[row, col].isOccupied)
                            cellInfos[row, col].SetHighlight(true, blockedColor);
                        else
                            cellInfos[row, col].SetHighlight(true, availableColor);
                    }
                    else
                        cellInfos[row, col].SetHighlight(false, availableColor);
                }
            }
        }

        private void ClearHighlight()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cellInfos[row, col] == null)
                        continue;

                    cellInfos[row, col].SetHighlight(false, availableColor);
                }
            }
        }

        /// <summary>
        /// dragLine이 없으면 생성하고, 기본 설정을 적용한다.
        /// </summary>
        private void EnsureDragLine()
        {
            dragLine = GetComponent<LineRenderer>();
            if (dragLine == null)
            {
                return;
            }

            dragLine.useWorldSpace = true;
            dragLine.positionCount = 2;
            dragLine.startWidth = dragLine.endWidth = 0.05f;
            dragLine.material = dragLine.material ?? new Material(Shader.Find("Sprites/Default"));
            dragLine.startColor = dragLine.endColor = dragPathColor;
            dragLine.sortingOrder = 2;
            dragLine.enabled = false;
        }

        /// <summary>
        /// 주어진 게임오브젝트에 해당하는 UnitCell을 찾는다.
        /// (cellInfos[row, col]를 순회)
        /// </summary>
        private UnitCell FindCellByGameObject(GameObject cell)
        {
            if (cell == null)
                return null;

            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cellInfos[row, col] != null && cellInfos[row, col].gameObject == cell)
                    {
                        return cellInfos[row, col];
                    }
                }
            }

            return null;
        }

        #region Cell Activation/Deactivation

        /// <summary>
        /// 특정 위치의 UnitCell을 활성화/비활성화합니다.
        /// </summary>
        /// <param name="row">행 인덱스</param>
        /// <param name="col">열 인덱스</param>
        /// <param name="active">활성화 여부 (true: 활성화, false: 비활성화)</param>
        /// <returns>성공 여부</returns>
        public bool SetCellActive(int row, int col, bool active)
        {
            // 범위 체크
            if (row < 0 || row >= gridHeight || col < 0 || col >= gridWidth)
            {
                Debug.LogWarning($"SetCellActive: 범위를 벗어난 인덱스입니다. row={row}, col={col}");
                return false;
            }

            if (cellInfos[row, col] == null)
            {
                Debug.LogWarning($"SetCellActive: 해당 위치에 UnitCell이 없습니다. row={row}, col={col}");
                return false;
            }

            // 비활성화하려는 셀에 유닛이 있는 경우 처리
            if (!active && cellInfos[row, col].isOccupied)
            {
                Debug.LogWarning($"SetCellActive: 유닛이 있는 셀은 비활성화할 수 없습니다. row={row}, col={col}");
                return false;
            }

            cellInfos[row, col].SetIsCellActive(active);
            return true;
        }

        /// <summary>
        /// GameObject로 UnitCell을 활성화/비활성화합니다.
        /// </summary>
        /// <param name="cell">UnitCell GameObject</param>
        /// <param name="active">활성화 여부 (true: 활성화, false: 비활성화)</param>
        /// <returns>성공 여부</returns>
        public bool SetCellActive(GameObject cell, bool active)
        {
            if (cell == null)
            {
                Debug.LogWarning("SetCellActive: cell이 null입니다.");
                return false;
            }

            UnitCell unitCell = FindCellByGameObject(cell);
            if (unitCell == null)
            {
                Debug.LogWarning($"SetCellActive: 해당 GameObject에 UnitCell이 없습니다. {cell.name}");
                return false;
            }

            // 비활성화하려는 셀에 유닛이 있는 경우 처리
            if (!active && unitCell.isOccupied)
            {
                Debug.LogWarning($"SetCellActive: 유닛이 있는 셀은 비활성화할 수 없습니다. {cell.name}");
                return false;
            }

            unitCell.SetIsCellActive(active);
            return true;
        }

        /// <summary>
        /// 특정 위치의 UnitCell 활성화 상태를 확인합니다.
        /// </summary>
        /// <param name="row">행 인덱스</param>
        /// <param name="col">열 인덱스</param>
        /// <returns>활성화 여부 (셀이 없으면 false)</returns>
        public bool IsCellActive(int row, int col)
        {
            if (row < 0 || row >= gridHeight || col < 0 || col >= gridWidth)
                return false;

            if (cellInfos[row, col] == null)
                return false;

            return cellInfos[row, col].isCellActive;
        }

        /// <summary>
        /// GameObject로 UnitCell 활성화 상태를 확인합니다.
        /// </summary>
        /// <param name="cell">UnitCell GameObject</param>
        /// <returns>활성화 여부 (셀이 없으면 false)</returns>
        public bool IsCellActive(GameObject cell)
        {
            if (cell == null)
                return false;

            UnitCell unitCell = FindCellByGameObject(cell);
            if (unitCell == null)
                return false;

            return unitCell.isCellActive;
        }

        /// <summary>
        /// 모든 UnitCell을 활성화/비활성화합니다.
        /// </summary>
        /// <param name="active">활성화 여부 (true: 활성화, false: 비활성화)</param>
        /// <param name="ignoreOccupied">유닛이 있는 셀도 비활성화할지 여부 (기본값: true, 유닛이 있는 셀은 건너뜀)</param>
        public void SetAllCellsActive(bool active, bool ignoreOccupied = true)
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cellInfos[row, col] == null)
                        continue;

                    // 비활성화하려는 경우 유닛이 있는 셀은 건너뛰기
                    if (!active && ignoreOccupied && cellInfos[row, col].isOccupied)
                        continue;

                    cellInfos[row, col].SetIsCellActive(active);
                }
            }
        }

        #endregion

        #region Unit Count Max Management

        /// <summary>
        /// 최대 유닛 수를 설정합니다. (런타임에서 동적으로 변경 가능)
        /// </summary>
        /// <param name="newMax">새로운 최대 유닛 수</param>
        public void SetUnitCountMax(int newMax)
        {
            if (newMax < 0)
            {
                Debug.LogWarning($"SetUnitCountMax: 최대 유닛 수는 0 이상이어야 합니다. (요청값: {newMax})");
                return;
            }

            if (unitCountMax != newMax)
            {
                unitCountMax = newMax;
                previousUnitCountMax = newMax;
                
                // 최대값이 줄어들었고 현재 유닛 수가 새로운 최대값을 초과하는 경우 처리
                if (unitCount > unitCountMax)
                {
                    Debug.LogWarning($"SetUnitCountMax: 최대 유닛 수가 {unitCountMax}로 줄어들었지만 현재 유닛 수({unitCount})가 더 많습니다.");
                }

                // 이벤트 발생하여 UI 업데이트
                OnUnitCountChanged?.Invoke(unitCount, unitCountMax);
            }
        }

        /// <summary>
        /// 최대 유닛 수를 증가시킵니다.
        /// </summary>
        /// <param name="amount">증가할 양</param>
        public void IncreaseUnitCountMax(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"IncreaseUnitCountMax: 증가량은 0보다 커야 합니다. (요청값: {amount})");
                return;
            }

            SetUnitCountMax(unitCountMax + amount);
        }

        /// <summary>
        /// 최대 유닛 수를 감소시킵니다.
        /// </summary>
        /// <param name="amount">감소할 양</param>
        public void DecreaseUnitCountMax(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"DecreaseUnitCountMax: 감소량은 0보다 커야 합니다. (요청값: {amount})");
                return;
            }

            SetUnitCountMax(Mathf.Max(0, unitCountMax - amount));
        }

        /// <summary>
        /// Inspector에서 값이 변경되었을 때 감지 (런타임에서도 작동)
        /// </summary>
        private void Update()
        {
            // unitCountMax가 Inspector에서 직접 변경되었는지 감지
            if (unitCountMax != previousUnitCountMax)
            {
                previousUnitCountMax = unitCountMax;
                OnUnitCountChanged?.Invoke(unitCount, unitCountMax);
            }
        }

        #endregion
    }
}