using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 최대한 왼쪽 위에서 시작해서 반시계 방향으로 활성 셀들의 바깥 경계를 따라 이동하는 경로를 생성
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
            //new Vector2Int(-1, -1),  // 1: 왼위
            new Vector2Int(0, -1),   // 2: 왼
            //new Vector2Int(1, -1),   // 3: 왼아래
            new Vector2Int(1, 0),    // 4: 아래 (row+1, col)
            //new Vector2Int(1, 1),    // 5: 오른아래
            new Vector2Int(0, 1),    // 6: 오른
            //new Vector2Int(-1, 1),   // 7: 오른위
        };

        public Vector3[] WorldRoute { get; private set; }
        public Vector3 startPosition { get; private set; }

        private Vector3[,] cellWorldPositions;

        [SerializeField]
        private TrailRenderer trailRenderer;

        [SerializeField]
        private float visualizationSpeed = 5f; // 경로 시각화 이동 속도

        private Coroutine visualizationCoroutine;

        public Action<Vector3[]> OnGenerateRoute;

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

            ConfigureTrailRenderer();
        }

        private void Start()
        {
            GenerateCellWorldPositions(GridUnitPlacement.Instance);
            GenerateBoundaryRoute();
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

            if (boundaryPath.Count == 0)
            {
                Debug.LogWarning("boundaryPath가 없습니다.");
                return new Vector3[0];
            }

            Debug.Log("boundaryPath: " + string.Join(", ", boundaryPath));

            // 그리드 좌표를 월드 좌표로 변환
            Vector3[] worldPath = ConvertToWorldPositions(boundaryPath);

            // 루트 시각화 필요
            VisualizeWorldPath(worldPath);

            WorldRoute = worldPath;
            startPosition = worldPath[0];

            OnGenerateRoute?.Invoke(worldPath);

            return worldPath;
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

                        // 주변 8방향향의 빈 공간을 체크

                        // 위
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

                        // 아래
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

                        // 왼쪽
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

                        // 오른쪽
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

                        // 왼쪽 위 대각선
                        if (row == 0 || col == 0)
                        {
                            var _pos = new Vector2Int(realPos.x - 1, realPos.y - 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row - 1, col - 1])
                        {
                            var _pos = new Vector2Int(realPos.x - 1, realPos.y - 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }

                        // 오른쪽 위 대각선
                        if (row == 0 || col == width - 1)
                        {
                            var _pos = new Vector2Int(realPos.x - 1, realPos.y + 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row - 1, col + 1])
                        {
                            var _pos = new Vector2Int(realPos.x - 1, realPos.y + 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }

                        // 왼쪽 아래 대각선
                        if (row == height - 1 || col == 0)
                        {
                            var _pos = new Vector2Int(realPos.x + 1, realPos.y - 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row + 1, col - 1])
                        {
                            var _pos = new Vector2Int(realPos.x + 1, realPos.y - 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }

                        // 오른쪽 아래 대각선
                        if (row == height - 1 || col == width - 1)
                        {
                            var _pos = new Vector2Int(realPos.x + 1, realPos.y + 1);
                            _posVertex.KeyValuePair[_pos] = new VertexNode { pos = _pos };
                        }
                        else if (!cellInfos[row + 1, col + 1])
                        {
                            var _pos = new Vector2Int(realPos.x + 1, realPos.y + 1);
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
        /// 그리드 좌표 -> 월드 좌표로 변환하는 표를 미리 생성
        /// </summary>
        private void GenerateCellWorldPositions(GridUnitPlacement grid)
        {
            cellWorldPositions = new Vector3[grid.gridHeight + 2, grid.gridWidth + 2];

            Vector3 basePosition = grid.cellInfos[0, 0].transform.position;

            // 이동 가능 범위는 왼쪽 상단으로 1칸 더 이동한 후 계산
            basePosition.x -= grid.cellSize;
            basePosition.y += grid.cellSize;

            for (int row = 0; row < grid.gridHeight + 2; row++)
            {
                for (int col = 0; col < grid.gridWidth + 2; col++)
                {
                    cellWorldPositions[row, col] = basePosition + new Vector3(col * grid.cellSize, -row * grid.cellSize, 0f);
                }
            }
        }

        /// <summary>
        /// 미리 생성한 표를 통해 그리드 좌표 -> 월드 좌표 변환합니다.
        /// </summary>
        private Vector3[] ConvertToWorldPositions(List<Vector2Int> gridPath)
        {
            List<Vector3> worldPath = new List<Vector3>();

            foreach (var gridPos in gridPath)
            {
                worldPath.Add(cellWorldPositions[gridPos.x, gridPos.y]);
            }

            Debug.Log("worldPath: " + string.Join(", ", worldPath));
            return worldPath.ToArray();
        }

        private void ConfigureTrailRenderer()
        {
            trailRenderer = GetComponentInChildren<TrailRenderer>();

            if (trailRenderer == null)
            {
                Debug.LogWarning("trailRenderer is null");
                return;
            }

            trailRenderer.time = 0.75f;
            trailRenderer.startWidth = 0.2f;
            trailRenderer.endWidth = 0.1f;
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            trailRenderer.startColor = Color.red;
            trailRenderer.endColor = new Color(1f, 0f, 0f, 0f);
        }

        private void VisualizeWorldPath(Vector3[] worldPath)
        {
            // worldPath의 좌표 요소들을 순서대로 보여주는 시각화

            if (worldPath == null || worldPath.Length == 0)
            {
                Debug.LogWarning("시각화할 경로가 없습니다.");
                return;
            }

            // 이전 시각화 코루틴이 실행 중이면 중지
            if (visualizationCoroutine != null)
            {
                StopCoroutine(visualizationCoroutine);
            }

            // Trail Renderer가 없으면 생성
            if (trailRenderer == null)
            {
                Debug.LogWarning("trailRenderer is null");
                return;
            }

            // 경로를 따라 이동하는 코루틴 시작
            visualizationCoroutine = StartCoroutine(FollowPathCoroutine(worldPath));
        }

        private IEnumerator FollowPathCoroutine(Vector3[] worldPath)
        {
            if (trailRenderer == null || worldPath.Length == 0)
                yield break;

            // 첫 번째 위치로 이동
            trailRenderer.transform.position = worldPath[0];

            // 트레일 초기화를 위해 잠시 대기
            yield return new WaitForSeconds(0.1f);

            // 경로를 따라 순차적으로 이동
            for (int i = 1; i < worldPath.Length; i++)
            {
                Vector3 startPos = worldPath[i - 1];
                Vector3 endPos = worldPath[i];
                float distance = Vector3.Distance(startPos, endPos);
                float travelTime = distance / visualizationSpeed;

                float elapsedTime = 0f;
                while (elapsedTime < travelTime)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / travelTime;
                    trailRenderer.transform.position = Vector3.Lerp(startPos, endPos, t);
                    yield return null;
                }

                // 정확한 위치로 설정
                trailRenderer.transform.position = endPos;
            }

            // 마지막 위치에서 트레일이 사라질 때까지 대기
            yield return new WaitForSeconds(trailRenderer.time);
        }
    }
}
