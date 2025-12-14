using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sangmin
{
    public class GridUnitPlacement : MonoBehaviour
    {
        public int gridWidth = 6;
        public int gridHeight = 4;
        public float cellSize = 1.0f;
        public Vector2 scale;
        public GameObject gridParent;
        public Color selectedColor = Color.yellow;
        public Color availableColor = Color.green;
        public Color blockedColor = Color.red;

        private UnitCell[,] cellInfos = new UnitCell[6, 4];
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

            // 1차원 배열(cellInfos)을 2차원 배열로 변경
            UnitCell[] unitCells = gridParent.GetComponentsInChildren<UnitCell>();
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    int index = x * gridHeight + y;
                    if (index < unitCells.Length)
                        cellInfos[x, y] = unitCells[index];
                }
            }
        }

        void Start()
        {

        }

        public void PlaceUnitFromFront(Unit unit)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (!cellInfos[x, y].isOccupied)
                    {
                        // UnitCell에 유닛을 배정하는 코드
                        cellInfos[x, y].PlaceUnit(unit);
                        return;
                    }
                }
            }
        }

        public void SelectCell(GameObject cell)
        {
            Debug.Log($"Selected cell: {cell.name}");

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (cell.Equals(cellInfos[x, y].gameObject))
                    {
                        // 셀 안에 유닛 있는지 확인, 확인이 되면 유닛이 있는 유의미한 셀을 선택한 것
                        if (cellInfos[x, y].isOccupied)
                            selectedCell = cellInfos[x, y];
                        else
                            return;
                    }
                }
            }

            // 유닛이 이미 놓여져 있는지 색깔로 여부 표시
            DrawHighlight();

            // 여기서부터 유닛 이동이든, 유닛 선택 시 사거리 표시 및 스테이터스 표시 등이 거능
        }

        public void UnSelectUnit()
        {
            selectedCell = null;
            ClearHighlight();
        }

        // 드래그 중에 이동할 칸의 표시를 할 필요가 있음

        public void MoveUnit(GameObject grid)
        {

        }

        private void DrawHighlight()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (cellInfos[x, y].Equals(selectedCell))
                        cellInfos[x, y].SetHighlight(true, selectedColor);
                    else
                    {
                        if (cellInfos[x, y].isOccupied)
                            cellInfos[x, y].SetHighlight(true, blockedColor);
                        else
                            cellInfos[x, y].SetHighlight(true, availableColor);
                    }
                }
            }
        }

        private void ClearHighlight()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    cellInfos[x, y].SetHighlight(false, availableColor);
                }
            }
        }

        private Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / cellSize);
            int y = Mathf.FloorToInt(worldPosition.y / cellSize);
            return new Vector2Int(x, y);
        }

        private Vector3 GetWorldPosition(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x * cellSize + cellSize / 2, gridPosition.y * cellSize + cellSize / 2, 0);
        }

        private bool IsValidGridPosition(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < gridWidth && gridPosition.y >= 0 && gridPosition.y < gridHeight;
        }
    }
}