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
        [field: SerializeField] public bool isOccupied{get; private set;}
        [SerializeField] private float cellSize = 1f;

        [Header("Visuals")]
        [SerializeField] private float lineWidth = 0.04f;
        [SerializeField, Range(0f,1f)] private float lineAlpha;
        [SerializeField, Range(0f,1f)] private float spriteAlpha;

        private LineRenderer lineRenderer;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            EnsureSpriteRenderer();
            EnsureLineRenderer();
            SetHighlight(false, Color.white);
        }

        public void Init(float size)
        {
            cellSize = size;
            EnsureSpriteRenderer();
            EnsureLineRenderer();
            SetHighlight(false, Color.white);
        }

        public void PlaceUnit(Unit _unit){
            unit = _unit;
            _unit.transform.position = transform.position;
            isOccupied = true;
        }

        /// <summary>
        /// 이 셀에 배치된 유닛을 반환 (없으면 null)
        /// </summary>
        public Unit GetUnit()
        {
            return unit;
        }

        /// <summary>
        /// 이 셀에서 유닛을 제거하고 비어 있는 상태로 만든다.
        /// </summary>
        public void ClearUnit()
        {
            unit = null;
            isOccupied = false;
        }

        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
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
    }
}