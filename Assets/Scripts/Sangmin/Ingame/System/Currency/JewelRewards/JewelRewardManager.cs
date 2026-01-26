using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 쥬얼 획득 규칙들을 설치/관리하는 매니저.
    /// - rules에 ScriptableObject 규칙들을 넣어두면 자동으로 동작한다.
    /// - rules가 비어있어도 기본 규칙들을 런타임에 생성해 적용할 수 있다.
    /// </summary>
    public class JewelRewardManager : MonoBehaviour
    {
        private static JewelRewardManager _instance;
        public static JewelRewardManager Instance => _instance;

        [Header("Rule Installation")]
        [Tooltip("추가/확장 가능한 규칙 리스트 (ScriptableObject)")]
        public List<JewelRewardRule> rules = new List<JewelRewardRule>();

        private JewelRewardContext _context;
        private readonly List<JewelRewardRule> _runtimeRules = new List<JewelRewardRule>();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            var currency = IngameCurrencyManager.Instance;
            if (currency == null)
            {
                Debug.LogError("[JewelRewardManager] IngameCurrencyManager.Instance가 없습니다. 쥬얼 보상 시스템이 비활성화됩니다.");
                return;
            }

            _context = new JewelRewardContext(currency);

            InstallAll();
        }

        private void OnDestroy()
        {
            UninstallAll();

            if (Instance == this)
                _instance = null;
        }

        private void InstallAll()
        {
            if (_context == null) return;

            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    if (rule == null) continue;
                    rule.Install(_context);
                }
            }

            foreach (var rule in _runtimeRules)
            {
                if (rule == null) continue;
                rule.Install(_context);
            }
        }

        private void UninstallAll()
        {
            if (_context == null) return;

            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    if (rule == null) continue;
                    rule.Uninstall(_context);
                }
            }

            foreach (var rule in _runtimeRules)
            {
                if (rule == null) continue;
                rule.Uninstall(_context);
            }
        }
    }
}

