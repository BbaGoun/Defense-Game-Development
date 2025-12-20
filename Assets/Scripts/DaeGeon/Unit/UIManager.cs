using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Popup")]
    public Transform popupRoot;          // Canvas 하위
    public GameObject unitInfoPopupPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void ShowUnitInfoPopup(UnitData data, UnitState state)
    {
        GameObject popupObj = Instantiate(unitInfoPopupPrefab, popupRoot);
        
        popupObj.SetActive(false);

        UnitInfoPopup ui = popupObj.GetComponent<UnitInfoPopup>();
        ui.Setup(data, state); 

        popupObj.SetActive(true);
    }
}
