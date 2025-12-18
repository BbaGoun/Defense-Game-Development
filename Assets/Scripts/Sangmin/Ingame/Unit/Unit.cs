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

        public void PerformAttack(Unit target)
        {
            attackBehaviour?.Attack(this, target);
        }

        public void OnCombatStart() // 랜덤 디펜스라 전투가 시작될 때 작동되는게 아님 - 수정 필요
        {
            foreach (var s in synergies)
                s.OnCombatStart(this);
        }
    }
}