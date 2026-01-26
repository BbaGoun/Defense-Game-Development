using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 유닛 등급에 따른 오라 색상을 제공하는 유틸리티 클래스
    /// </summary>
    public static class UnitGradeColor
    {
        /// <summary>
        /// 등급에 따른 오라 색상을 반환합니다.
        /// </summary>
        /// <param name="grade">유닛 등급</param>
        /// <returns>등급에 맞는 색상</returns>
        public static Color GetAuraColor(Grade grade)
        {
            return grade switch
            {
                Grade.NORMAL => Color.white,      // 흰색
                Grade.RARE => Color.blue,          // 파란색
                Grade.UNIQUE => new Color(0.5f, 0f, 0.5f, 1f), // 보라색 (RGB: 128, 0, 128)
                Grade.LEGEND => Color.yellow,      // 노란색
                Grade.MYTHIC => new Color(1f, 0.5f, 0f, 1f),   // 주황색 (RGB: 255, 128, 0)
                _ => Color.white
            };
        }
    }
}
