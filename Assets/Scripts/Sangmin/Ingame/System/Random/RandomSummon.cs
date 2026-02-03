using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 일반 뽑기와 특정 등급 뽑기를 모두 처리하는 시스템
    /// </summary>
    public class RandomSummon : MonoBehaviour
    {
        private static RandomSummon _instance;
        public static RandomSummon Instance
        {
            get
            {
                return _instance;
            }
        }

        [Header("사용 가능한 유닛 리스트")]
        [SerializeField] private List<GameObject> UnitList = new List<GameObject>();
        private Dictionary<UnitData, int> UnitPlacementCount = new Dictionary<UnitData, int>();

        [Header("등급별 유닛 리스트 (자동 생성)")]
        [SerializeField] private List<GameObject> normalUnitList = new List<GameObject>();
        [SerializeField] private List<GameObject> rareUnitList = new List<GameObject>();
        [SerializeField] private List<GameObject> uniqueUnitList = new List<GameObject>();
        [SerializeField] private List<GameObject> legendUnitList = new List<GameObject>();

        [SerializeField] private float normalSummonProbability;
        [SerializeField] private float rareSummonProbability;
        [SerializeField] private float uniqueSummonProbability;
        [SerializeField] private float legendSummonProbability;

        private bool gradeListsInitialized = false;

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
            gradeListsInitialized = false;

            foreach (var unit in UnitList)
            {
                UnitPlacementCount[unit.GetComponent<Unit>().unitData] = 0;
            }
        }

        public void AddToUnitList(GameObject unitPrefab)
        {
            UnitList.Add(unitPrefab);
            UnitPlacementCount[unitPrefab.GetComponent<Unit>().unitData] = 0;
        }

        public int GetUnitPlacementCount(UnitData unitData)
        {
            return UnitPlacementCount[unitData];
        }

        public void IncreaseUnitPlacementCount(UnitData unitData, int count)
        {
            UnitPlacementCount[unitData] += count;
        }

        public void ReduceUnitPlacementCount(UnitData unitData, int count)
        {
            UnitPlacementCount[unitData] -= count;
            if (UnitPlacementCount[unitData] < 0)
            {
                UnitPlacementCount[unitData] = 0;
            }
        }

        /// <summary>
        /// 일반 뽑기 (UnitList에서 랜덤 선택)
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit GetRandomUnit()
        {
            if (UnitList == null || UnitList.Count == 0)
            {
                Debug.LogWarning("[RandomSummon] 유닛 리스트가 비어있습니다!");
                return null;
            }

            float totalProbability = normalSummonProbability + rareSummonProbability + uniqueSummonProbability + legendSummonProbability;
            if (totalProbability <= 0f)
            {
                Debug.LogError("[RandomSummon] 소환 확률 총합이 0 이하입니다!");
                return null;
            }

            float rand = Random.value; // 0 이상 1 미만
            float cumulative = 0f;

            // 1. NORMAL
            cumulative += normalSummonProbability / totalProbability;
            if (rand < cumulative)
            {
                return GetUnitByGrade(Grade.NORMAL);
            }

            // 2. RARE
            cumulative += rareSummonProbability / totalProbability;
            if (rand < cumulative)
            {
                return GetUnitByGrade(Grade.RARE);
            }

            // 3. UNIQUE
            cumulative += uniqueSummonProbability / totalProbability;
            if (rand < cumulative)
            {
                return GetUnitByGrade(Grade.UNIQUE);
            }

            // 4. LEGEND
            cumulative += legendSummonProbability / totalProbability;
            if (rand <= cumulative)
            {
                return GetUnitByGrade(Grade.LEGEND);
            }

            Debug.Log("유닛 소환 오류: 어느 등급에도 포함되지 않음");
            return null;
        }

        /// <summary>
        /// 희귀 등급 유닛을 뽑습니다.
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit GetRareUnit()
        {
            return GetUnitByGrade(Grade.RARE);
        }

        /// <summary>
        /// 유니크(UNIQUE) 등급 유닛을 뽑습니다.
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit GetUniqueUnit()
        {
            return GetUnitByGrade(Grade.UNIQUE);
        }

        /// <summary>
        /// 전설 등급 유닛을 뽑습니다.
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit GetLegendUnit()
        {
            return GetUnitByGrade(Grade.LEGEND);
        }

        /// <summary>
        /// 등급별 유닛 리스트를 초기화합니다. (UnitList를 기반으로 자동 분류)
        /// </summary>
        private void InitializeGradeLists()
        {
            if (gradeListsInitialized) return;

            normalUnitList.Clear();
            rareUnitList.Clear();
            uniqueUnitList.Clear();
            legendUnitList.Clear();

            if (UnitList == null || UnitList.Count == 0)
            {
                Debug.LogWarning("[RandomSummon] UnitList가 비어있습니다! 등급별 리스트를 초기화할 수 없습니다.");
                return;
            }

            // UnitList의 각 프리팹에서 Unit 컴포넌트를 가져와 등급 확인
            foreach (GameObject unitPrefab in UnitList)
            {
                if (unitPrefab == null) continue;

                // 프리팹에서 Unit 컴포넌트 가져오기
                Unit unitComponent = unitPrefab.GetComponent<Unit>();
                if (unitComponent == null)
                {
                    Debug.LogWarning($"[RandomSummon] {unitPrefab.name}에 Unit 컴포넌트가 없습니다.");
                    continue;
                }

                // UnitData가 없으면 스킵
                if (unitComponent.unitData == null)
                {
                    Debug.LogWarning($"[RandomSummon] {unitPrefab.name}의 UnitData가 null입니다.");
                    continue;
                }

                // 등급에 따라 분류
                switch (unitComponent.unitData.grade)
                {
                    case Grade.NORMAL:
                        if (!normalUnitList.Contains(unitPrefab))
                            normalUnitList.Add(unitPrefab);
                        break;
                    case Grade.RARE:
                        if (!rareUnitList.Contains(unitPrefab))
                            rareUnitList.Add(unitPrefab);
                        break;
                    case Grade.UNIQUE:
                        if (!uniqueUnitList.Contains(unitPrefab))
                            uniqueUnitList.Add(unitPrefab);
                        break;
                    case Grade.LEGEND:
                        if (!legendUnitList.Contains(unitPrefab))
                            legendUnitList.Add(unitPrefab);
                        break;
                }
            }

            gradeListsInitialized = true;
            //Debug.Log($"[RandomSummon] 등급별 유닛 리스트 초기화 완료 - 일반: {normalUnitList.Count}, 희귀: {rareUnitList.Count}, 유니크: {uniqueUnitList.Count}, 전설: {legendUnitList.Count}");
        }

        /// <summary>
        /// 등급에 맞는 랜덤 유닛 프리팹을 반환합니다. (없으면 null)
        /// </summary>
        public GameObject GetRandomUnitPrefabByGrade(Grade grade)
        {
            InitializeGradeLists();

            List<GameObject> list = null;
            switch (grade)
            {
                case Grade.NORMAL: list = normalUnitList; break;
                case Grade.RARE: list = rareUnitList; break;
                case Grade.UNIQUE: list = uniqueUnitList; break;
                case Grade.LEGEND: list = legendUnitList; break;
                default: return null;
            }

            if (list == null || list.Count == 0) return null;
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// 등급에 맞는 랜덤 유닛을 소환합니다. (ObjectPool 사용)
        /// </summary>
        public Unit GetUnitByGrade(Grade grade)
        {
            GameObject prefab = GetRandomUnitPrefabByGrade(grade);
            if (prefab == null) return null;
            return prefab.GetComponent<Unit>();
        }

        /// <summary>
        /// 등급별 리스트를 강제로 재초기화합니다. (UnitList가 변경된 경우 호출)
        /// </summary>
        public void RefreshGradeLists()
        {
            gradeListsInitialized = false;
            InitializeGradeLists();
        }

        public Unit SummonUnit(Unit unit)
        {
            return ObjectPoolManager.Instance.GetObject(unit.gameObject).GetComponent<Unit>();
        }

        public Unit SummonUnit(GameObject unitPrefab)
        {
            return ObjectPoolManager.Instance.GetObject(unitPrefab).GetComponent<Unit>();
        }
    }
}
