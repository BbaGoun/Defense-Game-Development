using UnityEngine;

public class UnitUI : MonoBehaviour
{
    public static UnitUI Instance;

    [Header("ScrollView")]
    public Transform content;
    public GameObject unitPrefabUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        foreach (var data in UnitManager.Instance.allUnits)
            CreateUnitUI(data);
    }

void CreateUnitUI(UnitData data)
{
    if (unitPrefabUI == null)
    {
        Debug.LogError("unitPrefabUI is NULL");
        return;
    }

    if (content == null)
    {
        Debug.LogError("content is NULL");
        return;
    }

    if (UnitManager.Instance == null)
    {
        Debug.LogError("UnitManager.Instance is NULL");
        return;
    }

    GameObject obj = Instantiate(unitPrefabUI, content);
    UnitPrefabUI ui = obj.GetComponent<UnitPrefabUI>();

    if (ui == null)
    {
        Debug.LogError("UnitPrefabUI component missing on prefab");
        return;
    }

    UnitState state = UnitManager.Instance.GetState(data.unitId);
    ui.Setup(data, state);
}

}
