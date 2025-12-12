using UnityEngine;

namespace Sangmin
{
    public class UnitGrid : MonoBehaviour
    {
        [Header("Runtime state")]
        [SerializeField] private bool isOccupied;
        [SerializeField] private float cellSize = 1f;

        [Header("Visuals")]
        [SerializeField] private Color availableColor = Color.green;
        [SerializeField] private Color blockedColor = Color.red;
        [SerializeField] private float lineWidth = 0.04f;

        private LineRenderer lineRenderer;

        public bool IsOccupied => isOccupied;

        private void Awake()
        {
            EnsureLineRenderer();
        }

        public void Init(float size, Color available, Color blocked)
        {
            cellSize = size;
            availableColor = available;
            blockedColor = blocked;
            EnsureLineRenderer();
            SetHighlight(false);
        }

        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
        }

        public void SetHighlight(bool show, bool canPlace = true)
        {
            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.enabled = show;
            if (show)
            {
                lineRenderer.startColor = lineRenderer.endColor = canPlace ? availableColor : blockedColor;
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