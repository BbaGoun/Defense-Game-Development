using UnityEngine;

namespace Sangmin
{
    public class UnitCell : MonoBehaviour
    {
        [Header("Runtime state")]
        [SerializeField] private Unit unit;
        [field: SerializeField] public bool isOccupied{get; private set;}
        [SerializeField] private float cellSize = 1f;

        [Header("Visuals")]
        [SerializeField] private float lineWidth = 0.04f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            EnsureLineRenderer();
            SetHighlight(false, Color.white);
        }

        public void Init(float size, Color available, Color blocked)
        {
            cellSize = size;
            EnsureLineRenderer();
            SetHighlight(false, Color.white);
        }

        public void PlaceUnit(Unit _unit){
            unit = _unit;
        }

        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
        }

        public void SetHighlight(bool show, Color color)
        {
            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.enabled = show;
            if (show)
            {
                lineRenderer.startColor = lineRenderer.endColor = color;
            }
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
            lineRenderer.sortingOrder = 10;

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