using System.Collections.Generic;
using Sangmin;
using UnityEngine;

public class RandomSummon : MonoBehaviour
{
    private static RandomSummon _instance;
    public static RandomSummon Instance
    {
        get
        {
            return _instance;
        }
    }

    public List<GameObject> UnitList;

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

    public Unit SummonRandomUnit()
    {
        if (UnitList == null || UnitList.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, UnitList.Count);
        Unit selectedUnit = Instantiate(UnitList[randomIndex]).GetComponent<Unit>();
        return selectedUnit;
    }
}
