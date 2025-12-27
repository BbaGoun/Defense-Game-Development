using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Sangmin
{
    public class InputSystem : MonoBehaviour
    {
        private Camera _mainCamera;
        private PlayerInput _playerInput;
        private InputAction _clickAction;

        private bool _isPress;
        private bool _isDragging;

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
                    //_clickAction.started += OnClick;
                    _clickAction.performed += OnClick;
                    _clickAction.canceled += OnClick;
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
                _clickAction.canceled -= OnClick;
                _clickAction.Disable();
            }
        }

        private void OnClick(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                return;
            }
            else if (context.canceled)
            {
                _isPress = false;
                return;
            }
            //Debug.Log("Click");

            // UI 위에 마우스가 있는지 확인 (UI 버튼 클릭 시 무시)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
            if (!rayHit.collider)
            {
                if (GridUnitPlacement.Instance.isCellSelected)
                    GridUnitPlacement.Instance.UnSelectUnit();
                return;
            }

            if (rayHit.collider.CompareTag("Cell"))
            {
                if (rayHit.collider.gameObject != null)
                {
                    if (GridUnitPlacement.Instance.GetSelectedCell() != null && rayHit.collider.gameObject == GridUnitPlacement.Instance.GetSelectedCell().gameObject)
                    {
                        _isPress = true;
                        return;
                    }

                    if (GridUnitPlacement.Instance.SelectCell(rayHit.collider.gameObject))
                    {
                        // 유닛이 없는 Cell 클릭 시 선택 해제
                        GridUnitPlacement.Instance.UnSelectUnit();
                    }
                }
            }
        }

        private void Update()
        {
            if (_mainCamera == null || Mouse.current == null || GridUnitPlacement.Instance == null)
                return;

            // 마우스 왼쪽 버튼이 눌려 있는 동안 드래그 처리
            if (_isPress)
            {
                if (GridUnitPlacement.Instance.isCellSelected)
                {
                    var mousePos = Mouse.current.position.ReadValue();
                    var worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
                    worldPos.z = 0f;

                    // 드래그 시작 시점
                    if (!_isDragging)
                    {
                        _isDragging = true;
                        GridUnitPlacement.Instance.BeginDrag();
                    }

                    // 현재 마우스가 올라가 있는 셀 찾기
                    var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(mousePos));
                    GameObject hoverCell = null;
                    if (rayHit.collider && rayHit.collider.CompareTag("Cell"))
                    {
                        hoverCell = rayHit.collider.gameObject;
                    }

                    GridUnitPlacement.Instance.UpdateDrag(worldPos, hoverCell);
                }
            }
            else
            {
                // 버튼이 떼어질 때 드래그 종료 및 실제 이동 수행
                if (_isDragging)
                {
                    _isDragging = false;
                    GridUnitPlacement.Instance.EndDrag();
                }
            }
        }
    }
}
