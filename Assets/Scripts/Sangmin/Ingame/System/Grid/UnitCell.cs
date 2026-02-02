using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    public class UnitCell : MonoBehaviour
    {
        [Header("Grid Position")]
        public int row;
        public int col;
        [Header("Runtime state")]
        [SerializeField] private Unit unit;
        [SerializeField, Range(0, 3)] private int stackCount;
        [field: SerializeField] public bool isOccupied { get; private set; }
        [field: SerializeField] public bool isCellActive { get; private set; }
        [SerializeField] private float cellSize = 1f;

        [Header("Visuals")]
        [SerializeField] private float lineWidth = 0.04f;
        [SerializeField, Range(0f, 1f)] private float lineAlpha;
        [SerializeField, Range(0f, 1f)] private float spriteAlpha;

        private LineRenderer lineRenderer;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider2D;

        /// <summary>
        /// 스택 수를 시각적으로 보여주기 위한 추가 유닛(진짜 전투/시너지에는 사용되지 않음)
        /// </summary>
        private readonly List<GameObject> stackVisualUnits = new List<GameObject>();

        private void Awake()
        {
            EnsureSpriteRenderer();
            EnsureLineRenderer();
            EnsureCollider2D();
            SetHighlight(false, Color.white);

            isCellActive = true;
        }

        public void Init(float size)
        {
            cellSize = size;
            EnsureSpriteRenderer();
            EnsureLineRenderer();
            EnsureCollider2D();
            SetHighlight(false, Color.white);

            isCellActive = true;
        }

        public void PlaceUnit(Unit _unit)
        {
            if (_unit == null)
                return;

            unit = _unit;

            stackCount = 1;
            isOccupied = true;
            _unit.transform.position = transform.position;
            _unit.StackCount = stackCount;
            RefreshStackVisuals();
        }

        /// <summary>
        /// 이 셀에 배치된 유닛(대표 1개)을 반환 (없으면 null)
        /// </summary>
        public Unit GetUnit()
        {
            return unit;
        }

        public int GetStackCount()
        {
            return isOccupied ? stackCount : 0;
        }

        /// <summary>
        /// 같은 종류(=UnitData 동일)이고 등급이 NORMAL/RARE/UNIQUE인 경우에만 최대 3스택 가능
        /// </summary>
        public bool CanStack(Unit newUnit)
        {
            if (newUnit == null || newUnit.unitData == null) return false;
            if (!isOccupied) return false;
            if (unit == null || unit.unitData == null) return false;

            if (stackCount >= 3) return false;
            if (unit.unitData != newUnit.unitData) return false;

            Grade g = unit.unitData.grade;
            return g == Grade.NORMAL || g == Grade.RARE || g == Grade.UNIQUE;
        }

        /// <summary>
        /// 스택을 +1 한다. (실제 유닛 오브젝트는 1개만 유지)
        /// </summary>
        public void AddStackCount()
        {
            stackCount = Mathf.Clamp(stackCount + 1, 1, 3);
            if (unit != null) unit.StackCount = stackCount;

            RefreshStackVisuals();
        }

        public void SetStackCount(int count)
        {
            if (!isOccupied || unit == null)
            {
                stackCount = 0;
                RefreshStackVisuals();
                return;
            }
            stackCount = Mathf.Clamp(count, 1, 3);
            unit.StackCount = stackCount;
            RefreshStackVisuals();
        }

        /// <summary>
        /// 이 셀에서 유닛을 제거하고 비어 있는 상태로 만든다.
        /// </summary>
        public void ClearUnit()
        {
            unit = null;
            stackCount = 0;
            isOccupied = false;
            ClearStackVisuals();
        }

        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
        }

        public void SetIsCellActive(bool active)
        {
            isCellActive = active;

            UpdateCellVisual();
        }

        public void SetHighlight(bool show, Color color)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.enabled = show;
            if (show)
            {
                var c = new Color(color.r, color.g, color.b, spriteAlpha);
                spriteRenderer.color = c;
            }
        }

        private void EnsureSpriteRenderer()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }

            ConfigureSpriteRenderer();
        }

        private void ConfigureSpriteRenderer()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.sortingOrder = 1;
            spriteRenderer.material = spriteRenderer.material ?? new Material(Shader.Find("Sprites/Default"));

            var baseColor = new Color(1f, 1f, 1f, spriteAlpha);
            spriteRenderer.color = baseColor;
        }

        private void EnsureLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer == null)
                {
                    lineRenderer = gameObject.AddComponent<LineRenderer>();
                }
            }

            ConfigureLineRenderer();
        }

        private void ConfigureLineRenderer()
        {
            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.positionCount = 4;
            lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
            lineRenderer.numCornerVertices = 0;
            lineRenderer.numCapVertices = 0;
            lineRenderer.material = lineRenderer.material ?? new Material(Shader.Find("Sprites/Default"));
            lineRenderer.sortingOrder = 2;

            var baseColor = new Color(1f, 1f, 1f, lineAlpha);
            lineRenderer.startColor = lineRenderer.endColor = baseColor;

            var half = cellSize * 0.5f;
            lineRenderer.SetPositions(new[]
            {
                new Vector3(-half, half, 0f),
                new Vector3(half, half, 0f),
                new Vector3(half, -half, 0f),
                new Vector3(-half, -half, 0f),
            });
        }

        private void EnsureCollider2D()
        {
            if (boxCollider2D == null)
            {
                boxCollider2D = GetComponent<BoxCollider2D>();
                if (boxCollider2D == null)
                {
                    boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
                }

            }

            ConfigureBoxCollider2D();
        }

        private void ConfigureBoxCollider2D()
        {
            boxCollider2D.size = new Vector2(cellSize, cellSize);
            boxCollider2D.offset = Vector2.zero;
        }

        /// <summary>
        /// 셀의 스택 수에 맞춰 시각적 유닛들을 갱신한다.
        /// 실제 전투/시너지에는 영향을 주지 않고, 화면에 보이는 개수만 조절한다.
        /// </summary>
        private void RefreshStackVisuals()
        {
            ClearStackVisuals();

            if (!isOccupied || unit == null)
            {
                return;
            }


        }

        /// <summary>
        /// 모든 시각적 스택 유닛을 제거한다.
        /// </summary>
        private void ClearStackVisuals()
        {
        }

        private void UpdateCellVisual()
        {
            lineRenderer.enabled = isCellActive;
        }
    }
}