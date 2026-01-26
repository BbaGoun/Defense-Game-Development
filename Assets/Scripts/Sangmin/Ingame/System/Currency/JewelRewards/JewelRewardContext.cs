using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 쥬얼 보상 규칙이 접근할 수 있는 공용 컨텍스트.
    /// - CurrencyManager 접근
    /// - 런타임(한 판) 동안만 유지되는 상태 관리(Dictionary 기반)
    /// </summary>
    public sealed class JewelRewardContext
    {
        private readonly IngameCurrencyManager _currency;
        private readonly Dictionary<string, int> _intState = new Dictionary<string, int>();
        private readonly HashSet<string> _boolState = new HashSet<string>();

        public IngameCurrencyManager Currency => _currency;

        public JewelRewardContext(IngameCurrencyManager currency)
        {
            _currency = currency;
        }

        private string Key(string ruleId, string subKey)
        {
            return $"{ruleId}.{subKey}";
        }

        public int GetInt(string ruleId, string subKey, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(ruleId) || string.IsNullOrEmpty(subKey)) return defaultValue;
            string key = Key(ruleId, subKey);
            return _intState.TryGetValue(key, out int value) ? value : defaultValue;
        }

        public void SetInt(string ruleId, string subKey, int value)
        {
            if (string.IsNullOrEmpty(ruleId) || string.IsNullOrEmpty(subKey)) return;
            _intState[Key(ruleId, subKey)] = value;
        }

        public bool GetBool(string ruleId, string subKey, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(ruleId) || string.IsNullOrEmpty(subKey)) return defaultValue;
            string key = Key(ruleId, subKey);
            return _boolState.Contains(key) ? true : defaultValue;
        }

        public void SetBool(string ruleId, string subKey, bool value)
        {
            if (string.IsNullOrEmpty(ruleId) || string.IsNullOrEmpty(subKey)) return;
            string key = Key(ruleId, subKey);

            if (value) _boolState.Add(key);
            else _boolState.Remove(key);
        }

        public void AddJewel(int amount, string reason = null)
        {
            if (_currency == null) return;
            if (amount <= 0) return;
            _currency.AddJewel(amount);
            if (!string.IsNullOrEmpty(reason))
            {
                Debug.Log($"[JewelReward] +{amount} ({reason})");
            }
        }
    }
}

