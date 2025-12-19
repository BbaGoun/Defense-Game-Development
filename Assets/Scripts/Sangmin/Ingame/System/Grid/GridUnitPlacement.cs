using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sangmin
{
    public class GridUnitPlacement : MonoBehaviour
    {
        // 열(가로, column) 개수
        public int gridWidth = 6;
        // 행(세로, row) 개수
        public int gridHeight = 4;
        public float cellSize = 1.0f;
        public Vector2 scale;
        public GameObject gridParent;
        public Color selectedColor = Color.yellow;
        public Color availableColor = Color.green;
        public Color blockedColor = Color.red;

        // cellInfos[row, col] 형식으로 사용 (수학에서처럼 행 먼저, 열 나중)
        private UnitCell[,] cellInfos;
        private UnitCell selectedCell;
        public bool isCellSelected => selectedCell != null;

        [Header("Drag Move Settings")]
        public Color dragPathColor = Color.cyan;
        public Color dragTargetColor = Color.blue;
        [SerializeField] private LineRenderer dragLine;
        private UnitCell dragTargetCell;

        private static GridUnitPlacement _instance;
        public static GridUnitPlacement Instance
        {
            get { return _instance; }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);

            scale = new Vector2(transform.localScale.x, transform.localScale.y);

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

        public void PlaceUnitFromFront()
        {
            var unit = RandomSummon.Instance.SummonRandomUnit();

            // 행(row)을 먼저, 그 다음 열(col)을 순회
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (!cellInfos[row, col].isOccupied)
                    {
                        // UnitCell에 유닛을 배정하는 코드
                        cellInfos[row, col].PlaceUnit(unit);
                        return;
                    }
                }
            }
        }

        public bool SelectCell(GameObject cell)
        {
            Debug.Log($"Selected cell: {cell.name}");

            // (행, 열) 순서로 탐색
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cell.Equals(cellInfos[row, col].gameObject))
                    {
                        // 셀 안에 유닛 있는지 확인, 확인이 되면 유닛이 있는 유의미한 셀을 선택한 것
                        if (cellInfos[row, col].isOccupied)
                            selectedCell = cellInfos[row, col];
                        else
                            return true;
                    }
                }
            }

            Debug.Log("선택 완료");

            // 유닛이 이미 놓여져 있는지 색깔로 여부 표시
            DrawHighlight();

            // 여기서부터 유닛 이동이든, 유닛 선택 시 사거리 표시 및 스테이터스 표시 등이 거능
            return false;
        }

        public void UnSelectUnit()
        {
            Debug.Log("UnSelect");
            selectedCell = null;
            ClearHighlight();
        }

        // 드래그 중에 이동할 칸의 표시를 할 필요가 있음

        /// <summary>
        /// 선택된 셀의 유닛을 주어진 셀로 이동시킨다.
        /// </summary>
        public void MoveUnit(GameObject grid)
        {
            if (selectedCell == null || grid == null)
                return;

            UnitCell targetCell = FindCellByGameObject(grid);
            if (targetCell == null)
                return;

            // 이미 유닛이 있는 칸으로는 이동하지 않는다.
            if (targetCell.isOccupied)
                return;

            Unit movingUnit = selectedCell.GetUnit();
            if (movingUnit == null)
                return;

            // 원래 셀 비우고 새 셀로 이동
            selectedCell.ClearUnit();
            targetCell.PlaceUnit(movingUnit);

            // 선택 대상 셀을 새 위치로 갱신
            selectedCell = targetCell;

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

        private void DrawHighlight()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cellInfos[row, col].Equals(selectedCell))
                        cellInfos[row, col].SetHighlight(true, selectedColor);
                    else
                    {
                        if (cellInfos[row, col].isOccupied)
                            cellInfos[row, col].SetHighlight(true, blockedColor);
                        else
                            cellInfos[row, col].SetHighlight(true, availableColor);
                    }
                }
            }
        }

        private void ClearHighlight()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    cellInfos[row, col].SetHighlight(false, availableColor);
                }
            }
        }

        /// <summary>
        /// dragLine이 없으면 생성하고, 기본 설정을 적용한다.
        /// </summary>
        private void EnsureDragLine()
        {
            if (dragLine == null)
            {
                dragLine = GetComponent<LineRenderer>();
                if (dragLine == null)
                {
                    dragLine = gameObject.AddComponent<LineRenderer>();
                }
            }

            dragLine.useWorldSpace = true;
            dragLine.positionCount = 2;
            dragLine.startWidth = dragLine.endWidth = 0.05f;
            dragLine.material = dragLine.material ?? new Material(Shader.Find("Sprites/Default"));
            dragLine.startColor = dragLine.endColor = dragPathColor;
            dragLine.sortingOrder = 20;
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
    }
}