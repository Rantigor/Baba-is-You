using UnityEngine;
using System.Collections.Generic;
using System;
public class ObjectManager : MonoBehaviour
{
    [SerializeField]private List<GameObject> prefabObjects = new List<GameObject>();
    public Dictionary<Enum, GameObject> ObjectPrefabs;
    private void Start()
    {
        ObjectPrefabs = new Dictionary<Enum, GameObject>();
        foreach(GameObject obj in prefabObjects)
        {
            ObjectPrefabs.Add(obj.GetComponent<GridObjectBase>().GetObjectType(),obj);
        }
    }
}
