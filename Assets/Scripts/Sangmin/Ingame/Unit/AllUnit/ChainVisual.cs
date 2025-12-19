using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 단일 방향 체인(예: 위, 오른위 등)을 시각화하는 컴포넌트
    /// - SpriteRenderer를 이용해 연결 / 비연결 상태에 따라 다른 이미지를 보여준다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ChainVisual : MonoBehaviour
    {
        [Header("Sprites")]
        [Tooltip("체인이 존재하지만 아직 연결되지 않은 상태의 스프라이트")]
        public Sprite disconnectedSprite;

        [Tooltip("양쪽 유닛 체인이 서로 마주보고 연결된 상태의 스프라이트")]
        public Sprite connectedSprite;

        private SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 이 방향에 체인이 존재하는지 여부
        /// </summary>
        public void SetHasChain(bool hasChain, bool isConnected = false)
        {
            gameObject.SetActive(hasChain);

            if (!hasChain)
                return;

            SetConnected(isConnected);
        }

        /// <summary>
        /// 체인이 이미 존재한다고 가정하고, 연결/비연결 스프라이트 변경
        /// </summary>
        public void SetConnected(bool isConnected)
        {
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();

            if (isConnected && connectedSprite != null)
            {
                _renderer.sprite = connectedSprite;
            }
            else if (!isConnected && disconnectedSprite != null)
            {
                _renderer.sprite = disconnectedSprite;
            }
        }
    }
}


