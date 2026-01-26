using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sangmin
{
    public enum Grade
    {
        NORMAL,
        RARE,
        UNIQUE,
        LEGEND,
        MYTHIC
    }
    [CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
    public class UnitData : ScriptableObject
    {
        public float attackDamage;
        // 특정 수를 attackSpeed로 나눈 시간마다 공격이 가능하도록 함
        public float attackSpeed;
        // 칸 수 단위의 원의 반지름
        public int attackRange;
        public Grade grade;

        public GameObject attackEffect;

        [Header("SO 객체를 연결하여 사용")]
        public AttackBehaviour attackBehaviour;
        public List<Synergy> synergies = new List<Synergy>();
        [SerializeField]
        public List<IStatusEffect> statusEffects = new List<IStatusEffect>();

        [Header("광역 공격 사항, 해당되지 않으면 설정 No")]
        [Tooltip("광역 공격의 범위")]
        public float radius;

        [Header("투사체 공격 사항, 해당되지 않으면 설정 No")]
        [Tooltip("투사체 프리팹")]
        public GameObject projectilePrefab;

        [Tooltip("초당 이동 속도")]
        public float projectileSpeed = 12f;

        //[Tooltip("목표와 이 거리 이내로 접근하면 명중으로 처리")]
        //public float hitRadius = 0.1f;

        [Tooltip("목표에 닿지 못했을 때의 최대 생존 시간")]
        public float maxLifetime = 2f;

        [Header("버프 부여 사항, 해당되지 않으면 설정 No")]
        [Tooltip("공격력 버프량")]
        public float attackDamageBonus = 0f;
        [Tooltip("공격속도 버프량")]
        public float attackSpeedBonus = 0f;
    }
}
