using UnityEngine;
using UnityEngine.InputSystem;

namespace Sangmin
{
    public class InputSystem : MonoBehaviour
    {
        private Camera _mainCamera;

        void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if(!context.started) 
            {
                if(GridUnitPlacement.Instance.isUnitSelected)
                    GridUnitPlacement.Instance.UnSelectUnit();
                return;
            }
            var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
            if(!rayHit.collider)
            {
                if(GridUnitPlacement.Instance.isUnitSelected)
                    GridUnitPlacement.Instance.UnSelectUnit();
                return;
            }

            if(rayHit.collider.CompareTag("Grid")){
                if(GridUnitPlacement.Instance.isUnitSelected)
                    GridUnitPlacement.Instance.MoveUnit(rayHit.collider.gameObject);
                else
                    GridUnitPlacement.Instance.SelectGrid(rayHit.collider.gameObject);
            }
        }
    }
}
