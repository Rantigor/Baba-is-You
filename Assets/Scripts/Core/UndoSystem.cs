using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using NUnit.Framework.Internal.Filters;
using System.Linq;

public class UndoSystem : MonoBehaviour
{
    public Stack<Dictionary<GameObject,Vector2>> OldMaps = new Stack<Dictionary<GameObject,Vector2>>();
    private ObjectManager _objectManager;
    private RuleParser _RuleParser;
    public int RuleIndex = 0;
    private void Awake()
    {
        _objectManager = FindAnyObjectByType<ObjectManager>();
        _RuleParser = FindAnyObjectByType<RuleParser>();
    }
    public void Undo()
    {
        ReParse();
        ReMove();
        RuleIndex--;
    }
    public void ReMove()
    {
        if (OldMaps.Count == 0) return;
        Dictionary<GameObject, Vector2> lastTurnMap = OldMaps.Pop();
        foreach (KeyValuePair<GameObject, Vector2> kvp in lastTurnMap)
        {
            if (kvp.Key != null)
            {
                kvp.Key.GetComponent<GridObjectBase>().IsActive = true;
                kvp.Key.transform.position = kvp.Value;
            }
        }
        Physics2D.SyncTransforms();
    }
    public void ReParse()
    {
        List<GameRule> transformRules = new List<GameRule>();
        _RuleParser.RuleParse();
        RuleIndex--;
        List<GameRule> oldRules = _RuleParser.ActiveRules;
        foreach (GameRule rule in oldRules)
        {
            if (rule.Verb.Equals(VerbObject.Type.Is) && rule.Predicate is NounObject.Type)
            {
                transformRules.Add(rule);
            }
        }

        if (transformRules.Count == 0) return;

        GridObjectBase[] allObjects = FindObjectsByType<GridObjectBase>();

        foreach (GridObjectBase obj in allObjects)
        {
            if (obj.IsText) continue;
            int i = obj.ChanchedIndex;
            foreach (GameRule rule in transformRules)
            {
                if((i != RuleIndex)) continue;
                if (obj.EnumType.Equals(rule.Predicate) && obj.HasChanged)
                {
                    Instantiate(_objectManager.ObjectPrefabs[rule.Subject], obj.transform.position, Quaternion.identity);
                    Destroy(obj.gameObject);
                }
            }
        }
    }
}