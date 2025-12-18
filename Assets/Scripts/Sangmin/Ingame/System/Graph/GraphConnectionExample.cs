using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 방향 그래프(체인) + Union-Find 예시
    /// - 각 유닛은 8방향 중 2방향에만 간선을 가진다.
    /// - 마주 보는(양방향) 간선을 이용해 연결 컴포넌트를 계산한다.
    /// - 유닛 뽑기, 이동, 판매(삭제) 시 어떻게 동작하는지 예시를 보여준다.
    /// </summary>
    public class GraphConnectionExample : MonoBehaviour
    {
        /// <summary>
        /// 예시용 유닛 노드
        /// 실제 Unit 컴포넌트 대신, 간단히 id + 좌표 + 체인 비트로만 표현
        /// gridPos는 (행, 열) = (row, col) 순서로 사용한다.
        /// </summary>
        private class UnitNode
        {
            public int id;
            public Vector2Int gridPos;
        
            /// <summary>
            /// 8방향 비트마스크 (0~7 비트)
            /// 0: 위, 1: 오른위, 2: 오른, 3: 오른아래,
            /// 4: 아래, 5: 왼아래, 6: 왼, 7: 왼위 (시계 방향)
            /// </summary>
            public int chainMask;
        }

        // 현재 필드에 존재하는 유닛들 (id -> UnitNode)
        private readonly Dictionary<int, UnitNode> _units = new Dictionary<int, UnitNode>();
        // 좌표로 유닛 찾기용 (gridPos(row, col) -> id)
        private readonly Dictionary<Vector2Int, int> _posToUnitId = new Dictionary<Vector2Int, int>();
        
        // 예시용: 자동으로 증가하는 유닛 id
        private int _nextUnitId = 0;

        // 8방향 오프셋 (비트 인덱스와 동일한 순서)
        // (dr, dc) = (행 변화량, 열 변화량)
        private static readonly Vector2Int[] Dir8 =
        {
            new Vector2Int(-1, 0),   // 0: 위 (row-1, col)
            new Vector2Int(-1, 1),   // 1: 오른위
            new Vector2Int(0, 1),    // 2: 오른
            new Vector2Int(1, 1),    // 3: 오른아래
            new Vector2Int(1, 0),    // 4: 아래 (row+1, col)
            new Vector2Int(1, -1),   // 5: 왼아래
            new Vector2Int(0, -1),   // 6: 왼
            new Vector2Int(-1, -1),  // 7: 왼위
        };

        private void Start()
        {
            // 예시 시나리오 실행
            ExampleScenario();
        }

        /// <summary>
        /// 질문에서 말한 세 가지 상황을 순서대로 보여주는 예시 시나리오
        /// 1) 유닛 뽑기(생성)
        /// 2) 유닛 이동
        /// 3) 유닛 판매(삭제)
        /// </summary>
        private void ExampleScenario()
        {
            Debug.Log("===== 예시 시나리오 시작 =====");

            // 1. 유닛 뽑기 예시
            //   - (0,0)에 체인 방향: 위(0), 오른(2)
            //   - (0,1)에 체인 방향: 아래(4), 왼(6)
            //   - (1,0)에 체인 방향: 왼위(7), 위(0)
            //     gridPos는 (행, 열) 순서이므로,
            //     (0,0)은 0번째 행 0번째 열, (0,1)은 0번째 행 1번째 열을 의미한다.
            int u0 = SpawnUnit(new Vector2Int(0, 0), mask: (1 << 0) | (1 << 2));
            int u1 = SpawnUnit(new Vector2Int(0, 1), mask: (1 << 4) | (1 << 6));
            int u2 = SpawnUnit(new Vector2Int(1, 0), mask: (1 << 7) | (1 << 0));

            Debug.Log("=== 유닛 3개 뽑은 후 컴포넌트 ===");
            RebuildAndLogComponents();

            // 2. 유닛 이동 예시 (u2를 (1,1)으로 이동)
            MoveUnit(u2, new Vector2Int(1, 1));
            Debug.Log("=== u2를 (1,1)으로 이동 후 컴포넌트 ===");
            RebuildAndLogComponents();

            // 3. 유닛 판매(삭제) 예시 (u1 판매)
            SellUnit(u1);
            Debug.Log("=== u1을 판매(삭제) 후 컴포넌트 ===");
            RebuildAndLogComponents();

            Debug.Log("===== 예시 시나리오 끝 =====");
        }

        /// <summary>
        /// 1) 유닛 뽑기: 새로운 유닛을 필드에 추가
        /// - 위치와 체인 비트마스크(8방향 중 2방향)를 지정
        /// - 예: mask = (1<<0) | (1<<2) => 위, 오른 두 방향에 간선
        /// </summary>
        public int SpawnUnit(Vector2Int gridPos, int mask)
        {
            int id = _nextUnitId++;

            var node = new UnitNode
            {
                id = id,
                gridPos = gridPos,
                chainMask = mask
            };

            _units[id] = node;
            _posToUnitId[gridPos] = id;

            Debug.Log($"SpawnUnit: id={id}, pos={gridPos}, mask={System.Convert.ToString(mask, 2).PadLeft(8, '0')}");
            return id;
        }

        /// <summary>
        /// 2) 유닛 이동: 유닛의 그리드 위치만 바꾼다.
        /// - chainMask(방향 정보)는 그대로 두고, 좌표만 변경
        /// - 이동 후에는 인접 유닛이 달라지므로 그래프를 다시 만들고 컴포넌트를 다시 계산
        /// </summary>
        public void MoveUnit(int unitId, Vector2Int newPos)
        {
            if (!_units.TryGetValue(unitId, out var node))
            {
                Debug.LogWarning($"MoveUnit: id={unitId} 유닛이 존재하지 않습니다.");
                return;
            }

            // 기존 좌표 매핑 제거
            if (_posToUnitId.TryGetValue(node.gridPos, out var storedId) && storedId == unitId)
            {
                _posToUnitId.Remove(node.gridPos);
            }

            node.gridPos = newPos;
            _posToUnitId[newPos] = unitId;

            Debug.Log($"MoveUnit: id={unitId}, newPos={newPos}");
        }

        /// <summary>
        /// 3) 유닛 판매(삭제): 유닛을 완전히 제거
        /// - 유닛 딕셔너리와 좌표 매핑에서 제거
        /// - 이후 그래프/컴포넌트 재계산 시 이 유닛은 포함되지 않음
        /// </summary>
        public void SellUnit(int unitId)
        {
            if (!_units.TryGetValue(unitId, out var node))
            {
                Debug.LogWarning($"SellUnit: id={unitId} 유닛이 존재하지 않습니다.");
                return;
            }

            _units.Remove(unitId);
            if (_posToUnitId.TryGetValue(node.gridPos, out var storedId) && storedId == unitId)
            {
                _posToUnitId.Remove(node.gridPos);
            }

            Debug.Log($"SellUnit: id={unitId} 판매(삭제)");
        }

        /// <summary>
        /// 현재 필드에 있는 유닛들로부터 방향 그래프를 만들고,
        /// Union-Find로 \"마주보는\" 연결 컴포넌트들을 계산한 뒤 로그로 출력
        /// </summary>
        private void RebuildAndLogComponents()
        {
            // 1) 방향 그래프 만들기 (Dictionary<int, HashSet<int>>)
            Dictionary<int, HashSet<int>> directedGraph = BuildDirectedGraphFromUnits();

            // 2) Union-Find로 양방향(마주보는) 간선만 모아서 컴포넌트 계산
            UnionFind uf = new UnionFind();
            uf.BuildComponentsFromDirectedGraph(directedGraph);

            // 3) 모든 컴포넌트 출력
            Dictionary<int, List<int>> allComponents = uf.GetAllComponents();
            Debug.Log($"현재 연결된 컴포넌트 개수: {allComponents.Count}");
            int index = 1;
            foreach (var comp in allComponents.Values)
            {
                Debug.Log($"컴포넌트 {index}: [{string.Join(", ", comp)}]");
                index++;
            }
        }

        /// <summary>
        /// _units와 _posToUnitId를 이용해 방향 그래프를 생성
        /// - 각 유닛은 chainMask에 따라 최대 8방향 중 2방향으로 간선을 가짐
        /// - 실제로 간선이 이어지는지는 그 방향 칸에 유닛이 있는지로 판단
        /// </summary>
        private Dictionary<int, HashSet<int>> BuildDirectedGraphFromUnits()
        {
            Dictionary<int, HashSet<int>> graph = new Dictionary<int, HashSet<int>>();

            foreach (var kv in _units)
            {
                int id = kv.Key;
                UnitNode node = kv.Value;

                if (!graph.ContainsKey(id))
                    graph[id] = new HashSet<int>();

                // 체인 비트마스크를 보고 어떤 방향으로 간선을 가질지 결정
                for (int dirIndex = 0; dirIndex < 8; dirIndex++)
                {
                    int bit = 1 << dirIndex;
                    if ((node.chainMask & bit) == 0)
                        continue; // 이 방향은 선택되지 않음

                    Vector2Int neighborPos = node.gridPos + Dir8[dirIndex];
                    if (_posToUnitId.TryGetValue(neighborPos, out int neighborId))
                    {
                        // 해당 방향 칸에 유닛이 있으면 간선 추가
                        graph[id].Add(neighborId);
                    }
                }

                // 간선이 하나도 없더라도, 정점으로는 그래프에 존재해야 함
                if (!graph.ContainsKey(id))
                {
                    graph[id] = new HashSet<int>();
                }
            }

            return graph;
        }
    }
}