using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Sangmin
{
    public class InputSystem : MonoBehaviour
    {
        private Camera _mainCamera;
        private PlayerInput _playerInput;
        private InputAction _clickAction;
        private InputAction _doubleTapAction;

        private bool _isPress;
        private bool _isDragging;

        // 캐싱된 GraphicRaycaster 리스트
        private List<GraphicRaycaster> _cachedGraphicRaycasters = new List<GraphicRaycaster>();

        void Awake()
        {
            _mainCamera = Camera.main;
            _playerInput = GetComponent<PlayerInput>();

            // Player Input 컴포넌트가 없으면 추가
            if (_playerInput == null)
            {
                _playerInput = gameObject.AddComponent<PlayerInput>();
            }

            // 액션 찾기 및 구독
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

                _doubleTapAction = _playerInput.actions["DoubleTap"];
                if (_doubleTapAction != null)
                {
                    //_clickAction.started += OnClick;
                    _doubleTapAction.performed += OnDoubleTap;
                    _doubleTapAction.canceled += OnDoubleTap;
                    _doubleTapAction.Enable();
                }
            }
            else
            {
                Debug.LogError("Player Input 컴포넌트에 Input Action Asset이 할당되지 않았습니다.");
            }

            // Canvas와 GraphicRaycaster 캐싱
            CacheGraphicRaycasters();
        }

        /// <summary>
        /// 씬의 모든 Canvas에서 GraphicRaycaster를 찾아서 캐싱합니다.
        /// 나중에 씬을 이동하면 다시 실행해야 할 수도 있음
        /// </summary>
        private void CacheGraphicRaycasters()
        {
            _cachedGraphicRaycasters.Clear();
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

            foreach (Canvas canvas in canvases)
            {
                GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                if (raycaster != null)
                {
                    _cachedGraphicRaycasters.Add(raycaster);
                }
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
            if (_doubleTapAction != null)
            {
                _doubleTapAction.performed -= OnDoubleTap;
                _doubleTapAction.canceled -= OnDoubleTap;
                _doubleTapAction.Disable();
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
            if (IsPointerOverUI())
            {
                return;
            }

            // 모든 충돌체를 확인하여 Cell 태그를 우선적으로 선택
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));

            // Cell 태그를 가진 충돌체를 우선적으로 찾기
            RaycastHit2D cellHit = default;
            bool foundCell = false;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Cell"))
                {
                    cellHit = hit;
                    foundCell = true;
                    break; // Cell을 찾으면 즉시 중단
                }
            }

            // Cell을 찾지 못했으면 아무것도 선택하지 않음
            if (!foundCell)
            {
                if (GridUnitPlacement.Instance.isCellSelected)
                    GridUnitPlacement.Instance.UnSelectUnit();
                return;
            }

            // Cell을 찾았으면 선택 처리
            if (cellHit.collider.gameObject != null)
            {
                if (GridUnitPlacement.Instance.GetSelectedCell() != null && cellHit.collider.gameObject == GridUnitPlacement.Instance.GetSelectedCell().gameObject)
                {
                    _isPress = true;
                    return;
                }

                if (GridUnitPlacement.Instance.SelectCell(cellHit.collider.gameObject))
                {
                    // 유닛이 없는 Cell 클릭 시 선택 해제
                    GridUnitPlacement.Instance.UnSelectUnit();
                }
            }
        }

        private bool IsPointerOverUI()
        {
            // EventSystem이 없으면 UI가 없다고 판단
            EventSystem currentEventSystem = EventSystem.current;
            if (currentEventSystem == null)
                return false;

            // 캐싱된 GraphicRaycaster가 없으면 UI가 없다고 판단
            if (_cachedGraphicRaycasters == null || _cachedGraphicRaycasters.Count == 0)
                return false;

            // PointerEventData 생성
            PointerEventData pointerData = new PointerEventData(currentEventSystem)
            {
                position = Mouse.current.position.ReadValue()
            };

            // 캐싱된 GraphicRaycaster로 레이캐스트 수행
            List<RaycastResult> results = new List<RaycastResult>();

            foreach (GraphicRaycaster raycaster in _cachedGraphicRaycasters)
            {
                // GraphicRaycaster가 null이 아니고, 해당 Canvas가 활성화되어 있는지 확인
                if (raycaster != null && raycaster.gameObject.activeInHierarchy)
                {
                    raycaster.Raycast(pointerData, results);
                    if (results.Count > 0)
                        return true;
                }
            }

            return false;
        }

        private void OnDoubleTap(InputAction.CallbackContext context)
        {
            if (context.started)
                Debug.Log("Double Tap Start!!!");
            else if (context.performed)
                Debug.Log("Double Tap Perform!!!");
            else
                Debug.Log("Double Tap Cancel!!!");
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

                    // 현재 마우스가 올라가 있는 셀 찾기 (모든 충돌체 확인하여 Cell 우선 선택)
                    RaycastHit2D[] dragHits = Physics2D.GetRayIntersectionAll(_mainCamera.ScreenPointToRay(mousePos));
                    GameObject hoverCell = null;

                    // Cell 태그를 가진 충돌체를 우선적으로 찾기
                    foreach (RaycastHit2D hit in dragHits)
                    {
                        if (hit.collider != null && hit.collider.CompareTag("Cell"))
                        {
                            hoverCell = hit.collider.gameObject;
                            break; // Cell을 찾으면 즉시 중단
                        }
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
