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
        }

        void Start()
        {

        }

        public void PlaceUnitFromFront(Unit unit)
        {
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

        public void SelectCell(GameObject cell)
        {
            Debug.Log($"Selected cell: {cell.name}");

            // (행, 열) 순서로 탐색
            for (int row = 0; row < gridHeight; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (cell.Equals(cellInfos[row, col].gameObject))
                    {
                        selectedCell = cellInfos[row, col];
                        // // 셀 안에 유닛 있는지 확인, 확인이 되면 유닛이 있는 유의미한 셀을 선택한 것
                        // if (cellInfos[x, y].isOccupied)
                        //     selectedCell = cellInfos[x, y];
                        // else
                        //     return;
                    }
                }
            }

            Debug.Log("선택 완료");

            // 유닛이 이미 놓여져 있는지 색깔로 여부 표시
            DrawHighlight();

            // 여기서부터 유닛 이동이든, 유닛 선택 시 사거리 표시 및 스테이터스 표시 등이 거능
        }

        public void UnSelectUnit()
        {
            Debug.Log("UnSelect");
            selectedCell = null;
            ClearHighlight();
        }

        // 드래그 중에 이동할 칸의 표시를 할 필요가 있음

        public void MoveUnit(GameObject grid)
        {

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
    }
}