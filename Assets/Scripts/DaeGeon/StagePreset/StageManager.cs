using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace DaeGeon
{
    public class StageManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentParent;
    public GameObject stagePrefab;
    public List<StageData> stageDataList = new List<StageData>();
    public Button btnLeft;
    public Button btnRight;

    private int currentIdx = 0; // 현재 보고 있는 스테이지 인덱스
    private float targetPos = 0f; // 목표 스크롤 위치
    public float scrollSpeed = 5f; // 이동 속도

    void Start()
    {
        RefreshUI();
        UpdateScrollPosition();
    }

    // [버튼 연결용] 오른쪽 버튼
    public void OnClickNext()
    {
        if (currentIdx < stageDataList.Count - 1)
        {
            currentIdx++;
            UpdateScrollPosition();
        }
    }

    // [버튼 연결용] 왼쪽 버튼
    public void OnClickPrev()
    {
        if (currentIdx > 0)
        {
            currentIdx--;
            UpdateScrollPosition();
        }
    }

    void UpdateScrollPosition()
    {
        // 스테이지 개수에 따른 목표 위치 계산 (0 ~ 1 사이 값)
        // 스테이지가 3개라면: 0, 0.5, 1.0
        targetPos = (float)currentIdx / (stageDataList.Count - 1);
        
        // 코루틴으로 부드럽게 넘기기
        StopAllCoroutines();
        StartCoroutine(AnimateScroll());
        btnLeft.interactable = (currentIdx > 0);
        btnRight.interactable = (currentIdx < stageDataList.Count - 1);
    }

    IEnumerator AnimateScroll()
    {
        while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPos) > 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetPos, Time.deltaTime * scrollSpeed);
            yield return null;
        }
        scrollRect.horizontalNormalizedPosition = targetPos;
    }

    public void RefreshUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (StageData data in stageDataList)
        {
            GameObject obj = Instantiate(stagePrefab, contentParent);
            // 프리팹에 붙어있는 StageUIItem 컴포넌트를 가져와 데이터 전달
            StageUIItem uiItem = obj.GetComponent<StageUIItem>();
            if (uiItem != null)
            {
                uiItem.SetData(data);
            }
        }
    }
}
}