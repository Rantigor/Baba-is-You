using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class RuleParser : MonoBehaviour
{
    private RuleReader _ruleReader;
    private ObjectManager _objectManager;
    public List<GameRule> ActiveRules = new List<GameRule>();
    public List<GameObject> YouObjects = new List<GameObject>();
    public UndoSystem Undo;
    public bool HasObjectRule(Enum subject, Enum verb, Enum predicate)
    {
        GameRule obj = new GameRule(subject,verb,predicate);
        foreach(var rule in ActiveRules)
        {
            if(rule.Equals(obj))
            {
                return true;
            }
        }
        return false;
    }
    void Awake()
    {
        _ruleReader = FindAnyObjectByType<RuleReader>();
        _objectManager = FindAnyObjectByType<ObjectManager>();
        Undo = FindAnyObjectByType<UndoSystem>();
    }


    void Update()
    {

    }
    public void RuleSizeCheck()
    {
        if (_ruleReader.GetListOfRules().Count < 3)
        {
            _ruleReader.GetListOfRules().Clear();
        }
    }

    public void RuleParse()
    {
        _ruleReader.CheckRules();
        RuleSizeCheck();
        Parse();
        TransformationRulesApply();
        Interaction();
        _ruleReader.GetListOfRules().Clear();
    }

    public void Parse()
    {
        Undo.RuleIndex++;
        ActiveRules.Clear();
        List<List<GridObjectBase>> listOfRules = _ruleReader.GetListOfRules();
        foreach(List<GridObjectBase> line in listOfRules)
        {
            for(int i = 0; i < line.Count; i++)
            {
                if(line[i] is VerbObject)
                {
                    List<NounObject> validSubjects = GetSubjects(line, i);
                    List<GridObjectBase> validPredicates = GetPredicates(line, i);

                    if(validSubjects.Count > 0 && validPredicates.Count > 0)
                    {
                        foreach(NounObject validSubject in validSubjects)
                        {
                            foreach (GridObjectBase validPredicate in validPredicates)
                            {
                                RegisterRule(validSubject, (VerbObject)line[i], validPredicate);
                            }
                        }
                    }
                }
            }
        }
    }

    private List<NounObject> GetSubjects(List<GridObjectBase> line, int verbIndex)
    {
        List<NounObject> subjects = new List<NounObject>();
        bool expactingNoun = true;

        for(int i = verbIndex -1; i >= 0; i--)
        {
            if (expactingNoun && line[i] is NounObject)
            {
                subjects.Add((NounObject)line[i]);
                expactingNoun = false;
            }
            else if(!expactingNoun &&  (line[i] is ConjuctionObject))
            {
                expactingNoun=true;
            }
            else
            {
                break;
            }
        }

        return subjects;
    }

    private List<GridObjectBase> GetPredicates(List<GridObjectBase> line, int verbIndex)
    {
        List<GridObjectBase> predicates = new List<GridObjectBase>();
        bool expactingPredicates = true;

        for (int i = verbIndex + 1; i < line.Count; i++)
        {
            if (expactingPredicates && (line[i] is NounObject or PropertyObject))
            {
                predicates.Add(line[i]);
                expactingPredicates = false;
            }
            else if (!expactingPredicates && (line[i] is ConjuctionObject))
            {
                expactingPredicates = true;
            }
            else
            {
                break;
            }
        }

        return predicates;
    }

    private void RegisterRule(NounObject subject, VerbObject verb, GridObjectBase predicate)
    {
        GameRule newRule = new GameRule(subject.GetObjectType(), verb.GetObjectType(), predicate.GetObjectType());
        if (!ActiveRules.Contains(newRule))
        {
            ActiveRules.Add(newRule);
        }
    }   

    private void TransformationRulesApply()
    {
        List<GameRule> transformRules = new List<GameRule>();

        foreach (GameRule rule in ActiveRules)
        {
            if(rule.Verb.Equals(VerbObject.Type.Is) && rule.Predicate is NounObject.Type)
            {
                transformRules.Add(rule);
            }
        }

        if (transformRules.Count == 0) return;

        GridObjectBase[] allObjects = FindObjectsByType<GridObjectBase>();

        foreach (GridObjectBase obj in allObjects)
        {
            if (obj.IsText || !obj.IsActive) continue;
            foreach (GameRule rule in transformRules)
            {
                if (obj.EnumType.Equals(rule.Subject))
                {
                    GameObject currentObj = Instantiate(_objectManager.ObjectPrefabs[rule.Predicate], obj.transform.position, Quaternion.identity);
                    currentObj.GetComponent<GridObjectBase>().HasChanged = true;
                    currentObj.GetComponent<GridObjectBase>().ChanchedIndex = Undo.RuleIndex;
                    Destroy(obj.gameObject);
                }
            }
        }
    }
    public void Interaction()
    {
        YouObjects.Clear();
        GridObjectBase[] allObjects = FindObjectsByType<GridObjectBase>();

        foreach (GridObjectBase obj in allObjects)
        {
            if (obj.IsText) continue;
            if (obj.HasProperty(PropertyObject.Type.You))
            {
                YouObjects.Add(obj.gameObject);
            }
            Collider2D[] colliders = Physics2D.OverlapCircleAll(obj.transform.position, 0.1f);
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject != obj.gameObject)
                {
                    ObjectInteraction(obj, Physics2D.OverlapCircle(obj.transform.position, 0.1f).GetComponent<GridObjectBase>(), obj.DidInteracted);
                }
            }
        }
    }
    public void ObjectInteraction(GridObjectBase objA, GridObjectBase objB, bool didInteracted)
    {
        if (didInteracted || !objB.IsActive || !objA.IsActive) return;
        else didInteracted = true;
        if ((objA.HasProperty(PropertyObject.Type.Win) && objB.HasProperty(PropertyObject.Type.You)) ||
            (objB.HasProperty(PropertyObject.Type.Win) && objA.HasProperty(PropertyObject.Type.You)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (objB.HasProperty(PropertyObject.Type.Sink) || objA.HasProperty(PropertyObject.Type.Sink))
        {
            if (objA.IsText || objB.IsText) return;
            objB.ActiveacteControle(false);
            objB.transform.Translate(100,100,0);
            objA.ActiveacteControle(false);
            objA.transform.Translate(-100,100,0);
            return;
        }
        if (objB.HasProperty(PropertyObject.Type.Defeat) && objA.HasProperty(PropertyObject.Type.You))
        {
            objA.ActiveacteControle(false);
            objA.transform.Translate(-100, 100, 0);
            return;
        }
        else if (objA.HasProperty(PropertyObject.Type.Defeat) && objA.HasProperty(PropertyObject.Type.You))
        {
            objB.ActiveacteControle(false);
            objB.transform.Translate(100, 100, 0);
            return;
        }
        if (objB.HasProperty(PropertyObject.Type.Hot) && objA.HasProperty(PropertyObject.Type.Melt))
        {
            objA.ActiveacteControle(false);
            objA.transform.Translate(-100, 100, 0);
            return;
        }
        else if (objA.HasProperty(PropertyObject.Type.Hot) && objB.HasProperty(PropertyObject.Type.Melt))
        {
            objB.ActiveacteControle(false);
            objB.transform.Translate(100, 100, 0);
            return;
        }


    }
}

public struct GameRule
{
    public Enum Subject;
    public Enum Verb;
    public Enum Predicate;

    public GameRule(Enum subject, Enum verb, Enum predicate)
    {
        Subject = subject;
        Verb = verb;
        Predicate = predicate;
    }
}