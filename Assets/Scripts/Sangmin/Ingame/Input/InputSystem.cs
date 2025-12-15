using UnityEngine;
using UnityEngine.InputSystem;

namespace Sangmin
{
    public class InputSystem : MonoBehaviour
    {
        private Camera _mainCamera;
        private PlayerInput _playerInput;
        private InputAction _clickAction;

        void Awake()
        {
            _mainCamera = Camera.main;
            _playerInput = GetComponent<PlayerInput>();
            
            // Player Input 컴포넌트가 없으면 추가
            if (_playerInput == null)
            {
                _playerInput = gameObject.AddComponent<PlayerInput>();
            }

            // Click 액션 찾기 및 구독
            if (_playerInput.actions != null)
            {
                _clickAction = _playerInput.actions["Click"];
                if (_clickAction != null)
                {
                    _clickAction.performed += OnClick;
                    _clickAction.Enable();
                }
                else
                {
                    Debug.LogError("Click 액션을 찾을 수 없습니다. Input Action Asset에 'Click' 액션이 있는지 확인하세요.");
                }
            }
            else
            {
                Debug.LogError("Player Input 컴포넌트에 Input Action Asset이 할당되지 않았습니다.");
            }
        }

        void OnDestroy()
        {
            // 액션 구독 해제
            if (_clickAction != null)
            {
                _clickAction.performed -= OnClick;
                _clickAction.Disable();
            }
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            Debug.Log("Click");
            // performed 이벤트에 delegate방식으로 구독했으니, performed 때만 작동하도록 함.
            if(!context.performed) return;

            var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
            if(!rayHit.collider)
            {
                if(GridUnitPlacement.Instance.isCellSelected)
                    GridUnitPlacement.Instance.UnSelectUnit();
                return;
            }

            if(rayHit.collider.CompareTag("Cell")){
                GridUnitPlacement.Instance.SelectCell(rayHit.collider.gameObject);
            }
        }

        // 유닛의 이동을 드래그 방식으로 구현해야함
    }
}
