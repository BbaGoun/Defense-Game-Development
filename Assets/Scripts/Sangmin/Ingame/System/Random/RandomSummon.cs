using System.Collections.Generic;
using Sangmin;
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
        public List<GameObject> UnitList;

        [Header("등급별 유닛 리스트 (자동 생성)")]
        [SerializeField] private List<GameObject> rareUnitList = new List<GameObject>();
        [SerializeField] private List<GameObject> heroUnitList = new List<GameObject>();
        [SerializeField] private List<GameObject> legendUnitList = new List<GameObject>();

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
        }

        /// <summary>
        /// 일반 뽑기 (UnitList에서 랜덤 선택)
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit SummonRandomUnit()
        {
            if (UnitList == null || UnitList.Count == 0)
            {
                Debug.LogWarning("[RandomSummon] 유닛 리스트가 비어있습니다!");
                return null;
            }

            int randomIndex = Random.Range(0, UnitList.Count);
            GameObject unitPrefab = UnitList[randomIndex];

            if (unitPrefab == null)
            {
                Debug.LogError($"[RandomSummon] 유닛 프리팹이 null입니다! (인덱스: {randomIndex})");
                return null;
            }

            Unit selectedUnit = ObjectPoolManager.Instance.GetObject(unitPrefab).GetComponent<Unit>();
            return selectedUnit;
        }

        /// <summary>
        /// 희귀 등급 유닛을 뽑습니다.
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit SummonRareUnit()
        {
            InitializeGradeListsIfNeeded();

            if (rareUnitList == null || rareUnitList.Count == 0)
            {
                Debug.LogWarning("[RandomSummon] 희귀 등급 유닛 리스트가 비어있습니다!");
                return null;
            }

            int randomIndex = Random.Range(0, rareUnitList.Count);
            GameObject unitPrefab = rareUnitList[randomIndex];

            if (unitPrefab == null)
            {
                Debug.LogError($"[RandomSummon] 희귀 등급 유닛 프리팹이 null입니다! (인덱스: {randomIndex})");
                return null;
            }

            Unit selectedUnit = ObjectPoolManager.Instance.GetObject(unitPrefab).GetComponent<Unit>();
            return selectedUnit;
        }

        /// <summary>
        /// 영웅 등급 유닛을 뽑습니다.
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit SummonHeroUnit()
        {
            InitializeGradeListsIfNeeded();

            if (heroUnitList == null || heroUnitList.Count == 0)
            {
                Debug.LogWarning("[RandomSummon] 영웅 등급 유닛 리스트가 비어있습니다!");
                return null;
            }

            int randomIndex = Random.Range(0, heroUnitList.Count);
            GameObject unitPrefab = heroUnitList[randomIndex];

            if (unitPrefab == null)
            {
                Debug.LogError($"[RandomSummon] 영웅 등급 유닛 프리팹이 null입니다! (인덱스: {randomIndex})");
                return null;
            }

            Unit selectedUnit = ObjectPoolManager.Instance.GetObject(unitPrefab).GetComponent<Unit>();
            return selectedUnit;
        }

        /// <summary>
        /// 전설 등급 유닛을 뽑습니다.
        /// </summary>
        /// <returns>뽑은 유닛 (실패 시 null)</returns>
        public Unit SummonLegendUnit()
        {
            InitializeGradeListsIfNeeded();

            if (legendUnitList == null || legendUnitList.Count == 0)
            {
                Debug.LogWarning("[RandomSummon] 전설 등급 유닛 리스트가 비어있습니다!");
                return null;
            }

            int randomIndex = Random.Range(0, legendUnitList.Count);
            GameObject unitPrefab = legendUnitList[randomIndex];

            if (unitPrefab == null)
            {
                Debug.LogError($"[RandomSummon] 전설 등급 유닛 프리팹이 null입니다! (인덱스: {randomIndex})");
                return null;
            }

            Unit selectedUnit = ObjectPoolManager.Instance.GetObject(unitPrefab).GetComponent<Unit>();
            return selectedUnit;
        }

        /// <summary>
        /// 등급별 유닛 리스트를 초기화합니다. (UnitList를 기반으로 자동 분류)
        /// </summary>
        private void InitializeGradeListsIfNeeded()
        {
            if (gradeListsInitialized) return;

            rareUnitList.Clear();
            heroUnitList.Clear();
            legendUnitList.Clear();

            if (DaeGeon.UnitManager.Instance == null)
            {
                Debug.LogWarning("[RandomSummon] UnitManager.Instance가 없습니다! 등급별 리스트를 초기화할 수 없습니다.");
                return;
            }

            if (UnitList == null || UnitList.Count == 0)
            {
                Debug.LogWarning("[RandomSummon] UnitList가 비어있습니다! 등급별 리스트를 초기화할 수 없습니다.");
                return;
            }

            // UnitList의 각 프리팹을 UnitManager의 allUnits와 비교하여 등급 확인
            foreach (GameObject unitPrefab in UnitList)
            {
                if (unitPrefab == null) continue;

                // UnitManager에서 해당 프리팹과 일치하는 UnitData 찾기
                DaeGeon.UnitData matchingUnitData = null;
                foreach (var unitData in DaeGeon.UnitManager.Instance.allUnits)
                {
                    if (unitData.prefab == unitPrefab)
                    {
                        matchingUnitData = unitData;
                        break;
                    }
                }

                if (matchingUnitData == null) continue;

                // 등급에 따라 분류
                switch (matchingUnitData.grade)
                {
                    case DaeGeon.UnitGrade.RARE:
                        if (!rareUnitList.Contains(unitPrefab))
                            rareUnitList.Add(unitPrefab);
                        break;
                    case DaeGeon.UnitGrade.UNIQUE: // UNIQUE를 HERO로 매핑
                        if (!heroUnitList.Contains(unitPrefab))
                            heroUnitList.Add(unitPrefab);
                        break;
                    case DaeGeon.UnitGrade.LEGEND:
                        if (!legendUnitList.Contains(unitPrefab))
                            legendUnitList.Add(unitPrefab);
                        break;
                }
            }

            gradeListsInitialized = true;
            Debug.Log($"[RandomSummon] 등급별 유닛 리스트 초기화 완료 - 희귀: {rareUnitList.Count}, 영웅: {heroUnitList.Count}, 전설: {legendUnitList.Count}");
        }

        /// <summary>
        /// 등급별 리스트를 강제로 재초기화합니다. (UnitList가 변경된 경우 호출)
        /// </summary>
        public void RefreshGradeLists()
        {
            gradeListsInitialized = false;
            InitializeGradeListsIfNeeded();
        }
    }
}
