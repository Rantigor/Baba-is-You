using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class RuleReader : MonoBehaviour
{
    public List<GridObjectBase> TextObjects = new List<GridObjectBase>();
    private List<List<GridObjectBase>> ListOfRules = new List<List<GridObjectBase>>();
    public List<GridObjectBase> CurrentList = new List<GridObjectBase>();
    void Start()
    {

    }

    void Update()
    {
        
    }
    public void CheckRules()
    {
        ListOfRules.Clear();
        foreach (GridObjectBase obj in TextObjects)
        {
            if (obj.IsLeftEmpty())
            {

                obj.CheckRightGrid(ListOfRules, CurrentList);
            }
            if(obj.IsUpEmpty())
            {
                obj.CheckDownGrid(ListOfRules, CurrentList);
            }
        }

        foreach (List<GridObjectBase> obj in ListOfRules)
        {
            if(obj.Count < 3)
            {
                obj.Clear();
                continue;
            }
        }
    }
    public List<List<GridObjectBase>> GetListOfRules()
    {
        return ListOfRules;
    }
}
