using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// Grid의 활성 셀들(isCellActive)을 기반으로 테두리 경로를 자동 생성하는 유틸리티.
    /// 왼쪽 위에서 시작해서 반시계 방향으로 활성 셀들의 바깥 경계를 따라 이동하는 경로를 만듭니다.
    /// </summary>
    public class EnemyMoveRoute : MonoBehaviour
    {
        private static EnemyMoveRoute _instance;
        public static EnemyMoveRoute Instance
        {
            get
            {
                return _instance;
            }
        }

        [Serializable]
        private class VertexNode
        {
            public Vector2Int pos;

            [SerializeReference]
            public VertexNode inNode;
            [SerializeReference]
            public VertexNode outNode;
        }

        [Serializable]
        private class PosVertexDictionary : SerializableDictionary<Vector2Int, VertexNode> { }

        [SerializeField]
        private PosVertexDictionary _posVertex = new PosVertexDictionary();

        private static readonly Vector2Int[] Dir8 =
        {
            new Vector2Int(-1, 0),   // 0: 위 (row-1, col)
            new Vector2Int(-1, -1),  // 1: 왼위
            new Vector2Int(0, -1),   // 2: 왼
            new Vector2Int(1, -1),   // 3: 왼아래
            new Vector2Int(1, 0),    // 4: 아래 (row+1, col)
            new Vector2Int(1, 1),    // 5: 오른아래
            new Vector2Int(0, 1),    // 6: 오른
            new Vector2Int(-1, 1),   // 7: 오른위
        };

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// GridUnitPlacement의 현재 활성 셀 상태를 기반으로 테두리 경로를 생성합니다.
        /// </summary>
        /// <returns>월드 좌표 경로 (Vector3 배열). 경로가 없으면 빈 배열.</returns>
        public Vector3[] GenerateBoundaryRoute()
        {
            if (GridUnitPlacement.Instance == null)
            {
                Debug.LogWarning("GridUnitPlacement.Instance가 없습니다.");
                return new Vector3[0];
            }

            var grid = GridUnitPlacement.Instance;
            var cellInfos = GetCellInfos(grid);

            if (cellInfos == null)
                return new Vector3[0];

            // 활성 셀들의 경계 경로를 찾기
            List<Vector2Int> boundaryPath = FindBoundaryPath(cellInfos, grid.gridHeight, grid.gridWidth);

            Debug.Log("boundaryPath: " + string.Join(", ", boundaryPath));

            if (boundaryPath.Count == 0)
                return new Vector3[0];

            // 그리드 좌표를 월드 좌표로 변환
            //return ConvertToWorldPositions(boundaryPath, grid);
            // 루트 시각화 필요
            return new Vector3[0];
        }

        /// <summary>
        /// 활성 셀들의 테두리를 찾아서 반시계 방향 경로를 생성합니다.
        /// </summary>
        private List<Vector2Int> FindBoundaryPath(bool[,] cellInfos, int height, int width)
        {
            var path = new List<Vector2Int>();

            // 1. 이동 가능한 정점들을 추가
            AddVertexNode(cellInfos, height, width);

            // 2. 왼쪽 위에서 시작하는 시작점 찾기
            Vector2Int? startPoint = FindStartPoint();
            Debug.Log($"StartPoint: {startPoint}");

            // 3. startPoint를 시작으로 인접한 정점들을 서로 연결
            if (startPoint.HasValue)
                ConnectVertex(startPoint.Value, path);

            return path;
        }

        private void AddVertexNode(bool[,] cellInfos, int height, int width)
        {
            _posVertex.KeyValuePair.Clear();

            // 활성화 된 Cell 주변의 정점을 등록
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (cellInfos[row, col])
                    {
                        // grid를 감싸는 1칸까지가 이동 가능 영역이므로 행/열에 상하좌우 1칸씩이 추가.
                        // grid에서 [0,0]은 이동 가능 범위에서는 [1,1]임
                        var realPos = new Vector2Int(row + 1, col + 1);

                        // 상하좌우의 빈 공간을 체크

                        // 제일 윗 행은 윗 공간이 무조건 비어있음
                        if (row == 0)
                        {
                            var _pos = new Vector2Int(realPos.x - 1, realPos.y);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row - 1, col])
                        {
                            var _pos = new Vector2Int(realPos.x - 1, realPos.y);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }

                        // 제일 아래 행은 아랫 공간이 무조건 비어있음
                        if (row == height - 1)
                        {
                            var _pos = new Vector2Int(realPos.x + 1, realPos.y);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row + 1, col])
                        {
                            var _pos = new Vector2Int(realPos.x + 1, realPos.y);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }

                        // 제일 왼쪽 열은 왼쪽 공간이 무조건 비어있음
                        if (col == 0)
                        {
                            var _pos = new Vector2Int(realPos.x, realPos.y - 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row, col - 1])
                        {
                            var _pos = new Vector2Int(realPos.x, realPos.y - 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }

                        // 제일 오른쪽 열은 오른쪽 공간이 무조건 비어있음
                        if (col == width - 1)
                        {
                            var _pos = new Vector2Int(realPos.x, realPos.y + 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row, col + 1])
                        {
                            var _pos = new Vector2Int(realPos.x, realPos.y + 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                    }
                }
            }
        }

        private Vector2Int? FindStartPoint()
        {
            Vector2Int startPoint = new Vector2Int(1000, 1000);

            foreach (var kv in _posVertex.KeyValuePair)
            {
                Vector2Int pos = kv.Key;
                VertexNode node = kv.Value;

                // [2, 0]이 [0, 2]보다 우선시 되야해서 가중치를 조금 낮춤.
                // 행과 열의 차이가 너무 크면 별로니 가중치 조정
                double sumA = startPoint.x * 0.99 + startPoint.y + math.abs(startPoint.x - startPoint.y) * 0.01;
                double sumB = pos.x * 0.99 + pos.y + math.abs(pos.x - pos.y) * 0.01;
                if (sumA > sumB)
                {
                    startPoint.x = pos.x;
                    startPoint.y = pos.y;
                }
            }

            return startPoint;
        }

        private void ConnectVertex(Vector2Int startPoint, List<Vector2Int> path)
        {
            if (!_posVertex.KeyValuePair.ContainsKey(startPoint))
            {
                Debug.LogWarning($"ConnectVertex: startPoint={startPoint}이 이동 가능하지 않습니다.");
                return;
            }
            VertexNode startNode = _posVertex.KeyValuePair[startPoint];
            VertexNode currentNode = startNode;
            path.Add(startPoint);

            int count = 0;

            do
            {
                foreach (var dir in Dir8)
                {
                    if (!_posVertex.KeyValuePair.TryGetValue(currentNode.pos + dir, out var nextNode))
                        continue;
                    if (nextNode.outNode == currentNode) // 역주행 방지
                        continue;
                    currentNode.outNode = nextNode;
                    nextNode.inNode = currentNode;
                    path.Add(nextNode.pos);
                    currentNode = nextNode;
                    break;
                }
                count += 1;
            } while (currentNode != startNode && count < 1000);

            Debug.Log($"Count = {count}");
        }

        /// <summary>
        /// GridUnitPlacement에서 활성 셀 정보를 가져옵니다.
        /// </summary>
        private bool[,] GetCellInfos(GridUnitPlacement grid)
        {
            var cellInfos = grid.cellInfos;

            if (cellInfos == null)
                return null;

            bool[,] activeCells = new bool[grid.gridHeight, grid.gridWidth];

            for (int row = 0; row < grid.gridHeight; row++)
            {
                for (int col = 0; col < grid.gridWidth; col++)
                {
                    if (cellInfos[row, col] != null)
                    {
                        activeCells[row, col] = cellInfos[row, col].isCellActive;
                    }
                }
            }

            return activeCells;
        }

        /// <summary>
        /// 그리드 좌표 경로를 월드 좌표로 변환합니다.
        /// UnitCell의 실제 transform.position을 사용합니다.
        /// </summary>
        private Vector3[] ConvertToWorldPositions(List<Vector2Int> gridPath, GridUnitPlacement grid)
        {
            var cellInfos = GridUnitPlacement.Instance.cellInfos;

            if (cellInfos == null)
                return new Vector3[0];

            Vector3[] worldPath = new Vector3[gridPath.Count];

            for (int i = 0; i < gridPath.Count; i++)
            {
                Vector2Int gridPos = gridPath[i];
                int row = gridPos.y;
                int col = gridPos.x;

                // 범위 체크
                if (row >= 0 && row < grid.gridHeight && col >= 0 && col < grid.gridWidth)
                {
                    if (cellInfos[row, col] != null)
                    {
                        // UnitCell의 실제 위치 사용
                        worldPath[i] = cellInfos[row, col].transform.position;
                    }
                    else
                    {
                        // 셀이 없으면 계산된 위치 사용
                        worldPath[i] = CalculateWorldPosition(row, col, grid);
                    }
                }
                else
                {
                    // 맵 밖이면 계산된 위치 사용
                    worldPath[i] = CalculateWorldPosition(row, col, grid);
                }
            }

            return worldPath;
        }

        /// <summary>
        /// row, col을 월드 좌표로 변환합니다 (셀이 없을 때 사용).
        /// </summary>
        private Vector3 CalculateWorldPosition(int row, int col, GridUnitPlacement grid)
        {
            // 첫 번째 셀의 위치를 기준으로 계산
            var field = typeof(GridUnitPlacement).GetField("cellInfos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                var cellInfos = field.GetValue(grid) as UnitCell[,];
                if (cellInfos != null && cellInfos[0, 0] != null)
                {
                    Vector3 firstCellPos = cellInfos[0, 0].transform.position;
                    float offsetX = (col - 0) * grid.cellSize;
                    float offsetY = (0 - row) * grid.cellSize; // row가 증가하면 y는 감소
                    return firstCellPos + new Vector3(offsetX, offsetY, 0f);
                }
            }

            // 기본 계산 (중심 기준)
            float worldX = (col - grid.gridWidth * 0.5f + 0.5f) * grid.cellSize;
            float worldY = (grid.gridHeight * 0.5f - row - 0.5f) * grid.cellSize;
            return new Vector3(worldX, worldY, 0f);
        }
    }
}
