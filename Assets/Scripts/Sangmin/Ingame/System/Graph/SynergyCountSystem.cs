using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 방향 그래프(체인) + Union-Find 예시
    /// - 각 유닛은 8방향 중 2방향에만 간선을 가진다.
    /// - 마주 보는(양방향) 간선을 이용해 연결 컴포넌트를 계산한다.
    /// - 유닛 뽑기, 이동, 판매(삭제) 시 어떻게 동작하는지 예시를 보여준다.
    /// </summary>
    public class SynergyCountSystem : MonoBehaviour
    {
        private static SynergyCountSystem _instance;
        public static SynergyCountSystem Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 예시용 유닛 노드
        /// 실제 Unit 컴포넌트를 포함하여, id + 좌표 + 체인 비트로 표현
        /// gridPos는 (행, 열) = (row, col) 순서로 사용한다.
        /// </summary>
        [Serializable]
        private class UnitNode
        {

            public int id;
            public Vector2Int gridPos;

            /// <summary>
            /// 8방향 비트마스크 (0~7 비트)
            /// 0: 위, 1: 오른위, 2: 오른, 3: 오른아래,
            /// 4: 아래, 5: 왼아래, 6: 왼, 7: 왼위 (시계 방향)
            /// </summary>
            public Unit.ChainDirection chainMask;

            /// <summary>
            /// 실제 필드 위의 유닛 컴포넌트 참조
            /// 체인 연결 여부에 따라 시각화를 갱신할 때 사용된다.
            /// </summary>
            public Unit unit;
        }

        #region 직렬화
        [Serializable]
        private class IdToUnitNodeDictionary : SerializableDictionary<int, UnitNode>{}
        [Serializable]
        private class PosToIdDictionary : SerializableDictionary<Vector2Int, int>{}

        #endregion
        // 현재 필드에 존재하는 유닛들 (id -> UnitNode)
        [SerializeField]
        private IdToUnitNodeDictionary _units = new IdToUnitNodeDictionary();

        // 좌표로 유닛 찾기용 (gridPos(row, col) -> id)
        [SerializeField]
        private PosToIdDictionary _posToUnitId = new PosToIdDictionary();
        
        // 자동으로 증가하는 유닛 id
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

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);

        }

        private void Start()
        {

        }


        /// <summary>
        /// 1) 유닛 뽑기: 새로운 유닛을 필드에 추가
        /// - 위치와 체인 비트마스크(8방향 중 2방향)를 지정
        /// - 예: mask = (1<<0) | (1<<2) => 위, 오른 두 방향에 간선
        /// - 실제 Unit 참조를 함께 넘기면, 이후 체인 시각화를 자동으로 갱신할 수 있다.
        /// </summary>
        public int SpawnUnit(Vector2Int gridPos, int mask, Unit unit = null)
        {
            int id = _nextUnitId++;

            var node = new UnitNode
            {
                id = id,
                gridPos = gridPos,
                chainMask = (Unit.ChainDirection)mask,
                unit = unit
            };

            _units.KeyValuePair[id] = node;
            _posToUnitId.KeyValuePair[gridPos] = id;

            Debug.Log($"SpawnUnit: id={id}, pos={gridPos}, mask={System.Convert.ToString(mask, 2).PadLeft(8, '0')}");

            RebuildAndLogComponents();
            return id;
        }

        /// <summary>
        /// 2) 유닛 이동: 유닛의 그리드 위치만 바꾼다.
        /// - chainMask(방향 정보)는 그대로 두고, 좌표만 변경
        /// - 이동 후에는 인접 유닛이 달라지므로 그래프를 다시 만들고 컴포넌트를 다시 계산
        /// </summary>
        public void MoveUnit(Vector2Int pos, Vector2Int newPos)
        {
            if (!_posToUnitId.KeyValuePair.ContainsKey(pos))
            {
                Debug.LogWarning($"MoveUnit: pos={pos} 위치에 유닛이 존재하지 않습니다.");
                return;
            }
            int unitId = _posToUnitId.KeyValuePair[pos];
            if (!_units.KeyValuePair.TryGetValue(unitId, out var node))
            {
                Debug.LogWarning($"MoveUnit: id={unitId} 유닛이 존재하지 않습니다.");
                return;
            }

            // 기존 좌표 매핑 제거
            if (unitId == node.id)
            {
                _posToUnitId.KeyValuePair.Remove(node.gridPos);
            }

            node.gridPos = newPos;
            _posToUnitId.KeyValuePair[newPos] = unitId;

            Debug.Log($"MoveUnit: id={unitId}, newPos={newPos}");

            RebuildAndLogComponents();
        }

        /// <summary>
        /// 두 유닛의 위치를 교환한다.
        /// - aPos와 bPos에 각각 유닛이 있어야 함
        /// - 두 유닛의 gridPos를 교환하고 _posToUnitId 딕셔너리도 업데이트
        /// </summary>
        public void SwapUnit(Vector2Int aPos, Vector2Int bPos)
        {
            // 두 위치에 유닛이 있는지 확인
            if (!_posToUnitId.KeyValuePair.ContainsKey(aPos))
            {
                Debug.LogWarning($"SwapUnit: aPos={aPos} 위치에 유닛이 존재하지 않습니다.");
                return;
            }
            if (!_posToUnitId.KeyValuePair.ContainsKey(bPos))
            {
                Debug.LogWarning($"SwapUnit: bPos={bPos} 위치에 유닛이 존재하지 않습니다.");
                return;
            }

            int unitAId = _posToUnitId.KeyValuePair[aPos];
            int unitBId = _posToUnitId.KeyValuePair[bPos];

            // 같은 유닛이면 교환할 필요 없음
            if (unitAId == unitBId)
            {
                Debug.LogWarning($"SwapUnit: 같은 유닛입니다. 교환할 필요가 없습니다.");
                return;
            }

            if (!_units.KeyValuePair.TryGetValue(unitAId, out var nodeA))
            {
                Debug.LogWarning($"SwapUnit: id={unitAId} 유닛이 존재하지 않습니다.");
                return;
            }
            if (!_units.KeyValuePair.TryGetValue(unitBId, out var nodeB))
            {
                Debug.LogWarning($"SwapUnit: id={unitBId} 유닛이 존재하지 않습니다.");
                return;
            }

            // 기존 좌표 매핑 제거
            _posToUnitId.KeyValuePair.Remove(aPos);
            _posToUnitId.KeyValuePair.Remove(bPos);

            // 두 유닛의 위치 교환
            nodeA.gridPos = bPos;
            nodeB.gridPos = aPos;

            // 새 좌표 매핑 추가
            _posToUnitId.KeyValuePair[bPos] = unitAId;
            _posToUnitId.KeyValuePair[aPos] = unitBId;

            Debug.Log($"SwapUnit: id={unitAId}({aPos} -> {bPos}), id={unitBId}({bPos} -> {aPos})");

            RebuildAndLogComponents();
        }

        /// <summary>
        /// 3) 유닛 판매(삭제): 유닛을 완전히 제거
        /// - 유닛 딕셔너리와 좌표 매핑에서 제거
        /// - 이후 그래프/컴포넌트 재계산 시 이 유닛은 포함되지 않음
        /// </summary>
        public void SellUnit(Vector2Int pos)
        {
            if (!_posToUnitId.KeyValuePair.ContainsKey(pos))
            {
                Debug.LogWarning($"SellUnit: pos={pos} 위치에 유닛이 존재하지 않습니다.");
                return;
            }
            int unitId = _posToUnitId.KeyValuePair[pos];
            if (!_units.KeyValuePair.TryGetValue(unitId, out var node))
            {
                Debug.LogWarning($"SellUnit: id={unitId} 유닛이 존재하지 않습니다.");
                return;
            }

            // 기존 좌표 매핑 제거
            if (unitId == node.id)
            {
                _units.KeyValuePair.Remove(unitId);
                _posToUnitId.KeyValuePair.Remove(node.gridPos);
            }

            Debug.Log($"SellUnit: id={unitId} 판매(삭제)");

            RebuildAndLogComponents();
        }

        /// <summary>
        /// 현재 필드에 있는 유닛들로부터 방향 그래프를 만들고,
        /// Union-Find로 \"마주보는\" 연결 컴포넌트들을 계산한 뒤 로그로 출력
        /// 그리고 유닛들의 체인 연결 상태를 업데이트하여 시각화한다.
        /// </summary>
        private void RebuildAndLogComponents()
        {
            // 1) 방향 그래프 만들기 (Dictionary<int, HashSet<int>>)
            Dictionary<int, HashSet<int>> directedGraph = BuildDirectedGraphFromUnits();

            // 2) Union-Find로 양방향(마주보는) 간선만 모아서 컴포넌트 계산
            UnionFind uf = new UnionFind();
            uf.BuildComponentsFromDirectedGraph(directedGraph);

            // 3) 모든 컴포넌트 로그 출력
            Dictionary<int, List<int>> allComponents = uf.GetAllComponents();
            Debug.Log($"현재 연결된 컴포넌트 개수: {allComponents.Count}");
            int index = 1;
            foreach (var comp in allComponents.Values)
            {
                Debug.Log($"컴포넌트 {index}: [{string.Join(", ", comp)}]");
                index++;
            }

            // 4) 각 유닛의 체인 연결 여부를 계산하고, Unit/ChainVisual에 반영
            UpdateUnitChainConnections();
        }

        /// <summary>
        /// _units와 _posToUnitId를 이용해 방향 그래프를 생성
        /// - 각 유닛은 chainMask에 따라 최대 8방향 중 2방향으로 간선을 가짐
        /// - 실제로 간선이 이어지는지는 그 방향 칸에 유닛이 있는지로 판단
        /// </summary>
        private Dictionary<int, HashSet<int>> BuildDirectedGraphFromUnits()
        {
            Dictionary<int, HashSet<int>> graph = new Dictionary<int, HashSet<int>>();

            foreach (var kv in _units.KeyValuePair)
            {
                int id = kv.Key;
                UnitNode node = kv.Value;

                if (!graph.ContainsKey(id))
                    graph[id] = new HashSet<int>();

                // 체인 비트마스크를 보고 어떤 방향으로 간선을 가질지 결정
                for (int dirIndex = 0; dirIndex < 8; dirIndex++)
                {
                    int bit = 1 << dirIndex;
                    if (((int)node.chainMask & bit) == 0)
                        continue; // 이 방향은 선택되지 않음

                    Vector2Int neighborPos = node.gridPos + Dir8[dirIndex];
                    if (_posToUnitId.KeyValuePair.TryGetValue(neighborPos, out int neighborId))
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

        /// <summary>
        /// _units와 _posToUnitId, 각 유닛의 체인 비트마스크를 이용해
        /// "마주보는 체인"이 연결된 방향을 찾아 Unit/ChainVisual에 반영한다.
        /// </summary>
        private void UpdateUnitChainConnections()
        {
            // 1) 일단 모든 유닛의 체인을 "비연결" 상태 스프라이트로 초기화
            foreach (var kv in _units.KeyValuePair)
            {
                UnitNode node = kv.Value;
                Unit unit = node.unit;
                if (unit == null)
                    continue;

                foreach (Unit.ChainDirection dir in Enum.GetValues(typeof(Unit.ChainDirection)))
                {
                    // 0 플래그나 정의되지 않은 값은 무시
                    if (dir == 0)
                        continue;

                    if (!unit.chain.HasFlag(dir))
                        continue;

                    // 이 방향에 체인은 있지만, 아직 연결은 안 된 상태로 초기화
                    unit.SetChainConnectionState(dir, false);
                }
            }

            // 2) 실제로 "마주보는" 체인이 있는 쌍을 찾아 둘 다 연결 상태로 표시
            foreach (var kv in _units.KeyValuePair)
            {
                int id = kv.Key;
                UnitNode node = kv.Value;
                Unit unit = node.unit;
                if (unit == null)
                    continue;

                for (int dirIndex = 0; dirIndex < 8; dirIndex++)
                {
                    int bit = 1 << dirIndex;
                    Unit.ChainDirection dir = (Unit.ChainDirection)bit;

                    // 이 유닛이 이 방향으로 체인을 가지고 있지 않으면 패스
                    if ((node.chainMask & dir) == 0)
                        continue;

                    Vector2Int neighborPos = node.gridPos + Dir8[dirIndex];
                    if (!_posToUnitId.KeyValuePair.TryGetValue(neighborPos, out int neighborId))
                        continue;

                    if (!_units.KeyValuePair.TryGetValue(neighborId, out UnitNode neighborNode))
                        continue;

                    Unit neighborUnit = neighborNode.unit;
                    if (neighborUnit == null)
                        continue;

                    // 반대 방향(마주보는 방향) 비트 계산
                    Unit.ChainDirection oppositeDir = Unit.GetOppositeDirection(dir);

                    // 이웃 유닛이 그 반대 방향 체인을 가지고 있어야 "마주보는 체인"으로 인정
                    if ((neighborNode.chainMask & oppositeDir) == 0)
                        continue;

                    // 양쪽 모두 연결된 상태로 표시
                    unit.SetChainConnectionState(dir, true);
                    neighborUnit.SetChainConnectionState(oppositeDir, true);
                }
            }
        }
    }
}