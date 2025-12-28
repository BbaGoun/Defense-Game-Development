using UnityEngine;
using NavMeshPlus.Components;


namespace Sangmin
{
    public class NavMesh2D : MonoBehaviour
    {
        private static NavMesh2D _instance;
        public static NavMesh2D Instance
        {
            get
            {
                return _instance;
            }
        }

        [SerializeField] private NavMeshSurface surface;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        public void RebuildNavigation()
        {
            surface.BuildNavMesh();
        }
    }
}
