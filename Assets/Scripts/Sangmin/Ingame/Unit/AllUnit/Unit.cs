using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    [RequireComponent(typeof(PoolAble))]
    public class Unit : MonoBehaviour
    {
        /*
        유닛에 필요한거

        공격데미지
        공속
        공격 사거리
        체인 // 8방향을 8비트로 표현, 시계방향으로 셈 ex) 0000101 = 12시, 3시
        인접 시너지
            주변으로부터 내가 받을 것인가
            내가 주변에 줄 것인가
        롤체식 카운팅 + 체인 시너지
            체인이 연결 되었는가?
            체인 총 길이가 몇인가?
            시너지 효과는 어떻게 받는가?
            일단 체인을 판정하는 시스템은 만들었으니 유닛을 만들자.
            시너지 확정은 유닛의 생성, 이동, 삭제 시에 판정

        자체 추가효과(광역, 둔화) // 이건 상속 방식으로 하자.
        등급
        판매 가격(가치)
            등급별로 가격이 나눠질 것이기에 등급만 체크하면 됨
        중복 수 // 중복이 가능한 등급까지를 상속하자.
            영웅까지는 3개 겹치기 가능
        타입1(근거리,원거리,마법) // 타입 별 상속
        타입2(종족, 소속, 속성) // 시너지 별 상속
        */
        [Header("Base Stats")]
        public UnitStatData unitStatData;
        public float baseAttackDamage;
        public float baseAttackSpeed;
        public float baseAttackRange;

        [Header("Final Stats")]
        public float finalAttackDamage;
        public float finalAttackSpeed;
        public float finalAttackRange;
        // 실시간 스탯 변화를 계산할 필요가 있음
        public UnitStatData.Grade grade;

        // 체인 연결/비연결 시각화 필요
        public ChainDirection chain;
        [Flags]
        public enum ChainDirection
        {
            UP = 1 << 0,
            UPRIGHT = 1 << 1,
            RIGHT = 1 << 2,
            DOWNRIGHT = 1 << 3,
            DOWN = 1 << 4,
            DOWNLEFT = 1 << 5,
            LEFT = 1 << 6,
            UPLEFT = 1 << 7
        }

        public IAttackBehaviour attackBehaviour;
        public List<ISynergy> synergies = new List<ISynergy>();
        public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

        [Header("Chain Visuals (8 방향, 인덱스 0~7 = 위, 오른위, 오른, 오른아래, 아래, 왼아래, 왼, 왼위)")]
        public List<ChainVisual> chainVisuals = new List<ChainVisual>(8);

        private static readonly ChainDirection[] _allDirections =
        {
            ChainDirection.UP,
            ChainDirection.UPRIGHT,
            ChainDirection.RIGHT,
            ChainDirection.DOWNRIGHT,
            ChainDirection.DOWN,
            ChainDirection.DOWNLEFT,
            ChainDirection.LEFT,
            ChainDirection.UPLEFT
        };

        void Awake()
        {
            if (unitStatData == null)
            {
                Debug.LogError($"유닛 스탯이 배정되지 않음 : {gameObject.name}");
                return;
            }
            baseAttackDamage = unitStatData.attackDamage;
            baseAttackSpeed = unitStatData.attackSpeed;
            baseAttackRange = unitStatData.attackSpeed;
            grade = unitStatData.grade;
        }

        void OnEnable()
        {
            // 유닛이 생성될 때 8방향 중 2방향을 랜덤으로 선택하여 체인 부여
            InitializeRandomChains();
            // 방금 선택된 방향들에 대해 시각적으로 체인 생성 (초기에는 모두 "비연결" 상태)
            RefreshAllChainVisualsAsDisconnected();
        }

        public void PerformAttack(Unit target)
        {
            attackBehaviour?.Attack(this, target);
        }

        /*
        시너지가 발동되는 타이밍
        1회 변경 - 스탯 변화, 뭔가의 생성 등이 해당
        공격할 때 - 유닛의 공격 시 마다 발동, 공격 시 X%로 발동 등이 해당
        스택마다 발동 - 특정 조건 X 스택마다 발동 등이 해당
        시간마다 발동 - 특정 시간 X 주기마다 발동 등이 해당
        웨이브 시작 시 발동 - 새로운 웨이브가 시작될 때 발동이 해당
        웨이브 시작 후 X 초 후 발동 - 새로운 웨이브 몬스터가 충분히 쌓인 후 발동될 필요가 있는 경우 해당
        */

        public void OnChangeOnce()
        {
            // 이번에 조건이 충족된 얘들만 실행
            /* ex)
            foreach(var s in List of synergies)
                s.OnchangeOnce(this);
            */
        }

        public void OnAttack()
        {

        }

        public void OnStack()
        {

            // 특정 조건이 만족될 때 Stack을 쌓는 함수를 호출, 스택이 다 쌓이면 Stack 함수 내부에서 다른 행동을 또 호출하는 방식이 나을 듯
        }

        public void OnCooldownUp()
        {
            // 시간에 따른 특정 행동 실행
        }

        public void OnWaveStart()
        {
            foreach (var s in synergies)
                s.OnCombatStart(this);
        }

        #region Chain Logic

        /// <summary>
        /// 8방향 중 서로 다른 두 방향을 랜덤으로 선택해 체인 플래그를 설정한다.
        /// </summary>
        private void InitializeRandomChains()
        {
            // 혹시 에디터에서 미리 넣어둔 체인이 있다면 초기화
            chain = 0;

            // 0~7 인덱스 중에서 서로 다른 두 개를 뽑는다.
            int firstIndex = UnityEngine.Random.Range(0, _allDirections.Length);
            int secondIndex = firstIndex;
            while (secondIndex == firstIndex)
            {
                secondIndex = UnityEngine.Random.Range(0, _allDirections.Length);
            }

            ChainDirection firstDir = _allDirections[firstIndex];
            ChainDirection secondDir = _allDirections[secondIndex];
            
            chain |= firstDir;
            chain |= secondDir;
        }

        /// <summary>
        /// 현재 chain 플래그를 기준으로,
        /// - 체인이 없는 방향: 비활성화
        /// - 체인이 있는 방향: 활성화 + "비연결" 스프라이트로 표시
        /// </summary>
        private void RefreshAllChainVisualsAsDisconnected()
        {
            for (int i = 0; i < _allDirections.Length; i++)
            {
                ChainDirection dir = _allDirections[i];
                bool hasChain = chain.HasFlag(dir);
                SetChainVisual(dir, hasChain, false);
            }
        }

        /// <summary>
        /// 외부(예: 그래프/보드 시스템)에서 호출해서
        /// 특정 방향 체인이 연결되었는지/아닌지에 따라 스프라이트를 변경할 수 있는 메서드.
        /// </summary>
        public void SetChainConnectionState(ChainDirection direction, bool isConnected)
        {
            if (!chain.HasFlag(direction))
                return; // 이 방향에 애초에 체인이 없으면 무시

            SetChainVisual(direction, true, isConnected);
        }

        /// <summary>
        /// 단일 방향 체인 시각화 오브젝트에 "체인 존재 여부"와 "연결 여부"를 반영.
        /// </summary>
        private void SetChainVisual(ChainDirection direction, bool hasChain, bool isConnected)
        {
            ChainVisual visual = GetChainVisual(direction);
            if (visual == null)
                return;

            visual.SetHasChain(hasChain, isConnected);
        }

        /// <summary>
        /// enum 방향 값에 대응되는 ChainVisual 반환
        /// (인스펙터에서 chainVisuals[0~7]에 할당하는 방식)
        /// </summary>
        private ChainVisual GetChainVisual(ChainDirection direction)
        {
            int index = GetDirectionIndex(direction);
            if (index < 0)
                return null;

            if (chainVisuals == null || chainVisuals.Count <= index)
                return null;

            return chainVisuals[index];
        }

        /// <summary>
        /// 어떤 방향의 반대(마주보는) 방향을 구하는 유틸 함수.
        /// Graph/보드 시스템에서 이 함수를 사용하면 방향 매핑이 꼬이지 않는다.
        /// </summary>
        public static ChainDirection GetOppositeDirection(ChainDirection direction)
        {
            int index = GetDirectionIndexStatic(direction);
            if (index < 0)
                return direction;

            int oppositeIndex = (index + 4) % 8;
            return _allDirections[oppositeIndex];
        }

        /// <summary>
        /// 단일 방향 플래그가 몇 번째 비트(0~7)인지 반환 (인스턴스용)
        /// </summary>
        private int GetDirectionIndex(ChainDirection direction)
        {
            return GetDirectionIndexStatic(direction);
        }

        /// <summary>
        /// 단일 방향 플래그가 몇 번째 비트(0~7)인지 반환 (static 유틸)
        /// 값이 0이거나 복수 비트면 -1 반환
        /// </summary>
        private static int GetDirectionIndexStatic(ChainDirection direction)
        {
            int value = (int)direction;
            if (value == 0)
                return -1;

            int index = 0;
            // value는 2의 거듭제곱(단일 플래그)라고 가정
            while ((value & 1) == 0)
            {
                value >>= 1;
                index++;
            }

            // 0~7 범위 밖이면 잘못된 값
            if (index < 0 || index >= 8)
                return -1;

            return index;
        }

        #endregion

    }
}