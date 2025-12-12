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
        public GameObject gridParent;
        public Color availableColor = Color.green;
        public Color blockedColor = Color.red;

        private GameObject[,] grids = new GameObject[6, 4];
        private UnitGrid[,] cellInfos = new UnitGrid[6, 4];
        private Unit selectedUnit;
        private UnitGrid selectedGrid;
        public bool isUnitSelected => selectedUnit != null;
        private UnitGrid highlightedCell;

        private static GridUnitPlacement _instance;
        public static GridUnitPlacement Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GridUnitPlacement>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("GridUnitPlacement");
                        _instance = obj.AddComponent<GridUnitPlacement>();
                    }
                }
                return _instance;
            }
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
            // 1차원 배열(cellInfos)을 2차원 배열로 변경
            UnitGrid[] unitGrids = gridParent.GetComponentsInChildren<UnitGrid>();
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    int index = x * gridHeight + y;
                    if (index < unitGrids.Length)
                    {
                        grids[x, y] = unitGrids[index].gameObject;
                        cellInfos[x, y] = unitGrids[index];
                    }
                }
            }
        }

        void Start()
        {

        }

        public void SelectGrid(GameObject grid)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid.Equals(grids[x,y]))
                    {
                        selectedGrid = cellInfos[x,y];
                    }
                }
            }
            // 그리드 선택 후 안에 유닛 있는지 확인, 확인이 되면 유닛이 있는 유의미한 그리드를 선택한 것으로 상태를 변경
            // 여기서부터 유닛 이동이든, 유닛 선택 시 사거리 표시 및 스테이터스 표시 등이 거능
            // selectedUnit = grid;
            // UpdateHighlight(posX, posY);
        }

        public void UnSelectUnit()
        {

        }

        public void MoveUnit(GameObject grid)
        {

        }

        void Update()
        {


            // if (Input.GetMouseButton(0) && selectedUnit != null)
            // {
            //     Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //     selectedUnit.transform.position = new Vector3(mousePosition.x, mousePosition.y, selectedUnit.transform.position.z);
            //     Vector2Int hoverGrid = GetGridPosition(mousePosition);
            //     UpdateHighlight(hoverGrid);
            // }

            // if (Input.GetMouseButtonUp(0) && selectedUnit != null)
            // {
            //     Vector2Int gridPosition = GetGridPosition(selectedUnit.transform.position);

            //     if (IsValidGridPosition(gridPosition))
            //     {
            //         if (grids[gridPosition.x, gridPosition.y] != null)
            //         {
            //             GameObject existingUnit = grids[gridPosition.x, gridPosition.y];
            //             grids[selectedUnitOriginalPosition.x, selectedUnitOriginalPosition.y] = existingUnit;
            //             existingUnit.transform.position = GetWorldPosition(selectedUnitOriginalPosition);
            //             cellInfos[selectedUnitOriginalPosition.x, selectedUnitOriginalPosition.y]?.SetOccupied(true);
            //         }
            //         else
            //         {
            //             grids[selectedUnitOriginalPosition.x, selectedUnitOriginalPosition.y] = null;
            //             cellInfos[selectedUnitOriginalPosition.x, selectedUnitOriginalPosition.y]?.SetOccupied(false);
            //         }

            //         grids[gridPosition.x, gridPosition.y] = selectedUnit;
            //         selectedUnit.transform.position = GetWorldPosition(gridPosition);
            //         cellInfos[gridPosition.x, gridPosition.y]?.SetOccupied(true);
            //     }
            //     else
            //     {
            //         selectedUnit.transform.position = GetWorldPosition(selectedUnitOriginalPosition);
            //     }

            //     selectedUnit = null;
            //     ClearHighlight();
            // }
        }

        private void UpdateHighlight(Vector2Int gridPosition)
        {
            ClearHighlight();

            if (!IsValidGridPosition(gridPosition))
            {
                return;
            }

            var cell = cellInfos[gridPosition.x, gridPosition.y];
            if (cell == null)
            {
                return;
            }

            bool canPlace = grids[gridPosition.x, gridPosition.y] == null;
            cell.SetHighlight(true, canPlace);
            highlightedCell = cell;
        }

        private void ClearHighlight()
        {
            if (highlightedCell != null)
            {
                highlightedCell.SetHighlight(false);
                highlightedCell = null;
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