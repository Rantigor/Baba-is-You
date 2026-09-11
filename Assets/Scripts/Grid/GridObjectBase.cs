using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public abstract class GridObjectBase : MonoBehaviour
{
    protected RuleParser _ruleParser;
    private UndoSystem _undoSystem;
    private void Awake()
    {
        _ruleParser = FindAnyObjectByType<RuleParser>();
        textLayerMask = LayerMask.GetMask("TextObject");
        _undoSystem = FindAnyObjectByType<UndoSystem>();
        if (isText)
        {
            FindAnyObjectByType<RuleReader>().TextObjects.Add(this);
        }
    }
    public abstract Enum EnumType { get; }
    [SerializeField] protected bool isText = false;
    public bool IsText => isText;
    public int textLayerMask;
    public bool DidInteracted = false;

    [Header("Obje Özellikleri")]
    public bool CanMove => (isText) ? true : !_ruleParser.HasObjectRule(EnumType, VerbObject.Type.Is, PropertyObject.Type.Stop);
    public bool IsPushable => (isText)? true:_ruleParser.HasObjectRule(EnumType,VerbObject.Type.Is,PropertyObject.Type.Push);
    public bool IsActive = true;
    public bool HasChanged = false;
    public int ChanchedIndex = 0;
    public void ActiveacteControle(bool decision)
    {
        IsActive = decision;
    }

    protected virtual void Start()
    {
        
    }
    public bool IsLeftEmpty()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + Vector3.left, Vector2.left, .1f, textLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                continue;
            }
            return false;
        }
        return true;
    }
    public void CheckRightGrid(List<List<GridObjectBase>> listOfRules, List<GridObjectBase> currentList)
    {
        if(!IsActive) return;  
        currentList.Add(this);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + Vector3.right, Vector2.right, .1f, textLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                hit.collider.GetComponent<GridObjectBase>().CheckRightGrid(listOfRules, currentList);
                return;
            }
        }
        listOfRules.Add(new List<GridObjectBase>(currentList));
        currentList.Clear();
    }
    public bool IsUpEmpty()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + Vector3.up, Vector2.up, .1f, textLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                continue;
            }
            return false;
        }
        return true;
    }
    public void CheckDownGrid(List<List<GridObjectBase>> listOfRules, List<GridObjectBase> currentList)
    {
        if (!IsActive) return;
        currentList.Add(this);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + Vector3.down, Vector2.down, .1f, textLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                hit.collider.GetComponent<GridObjectBase>().CheckDownGrid(listOfRules, currentList);
                return;
            }
        }
        listOfRules.Add(new List<GridObjectBase>(currentList));
        currentList.Clear();
    }

    public bool MoveControl(Vector2 movedir, Dictionary<GameObject,Vector2> currentMap)
    {
        if (!IsActive || !CanMove) return false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + (Vector3)movedir, movedir, .1f);
        bool _canMove = true;

        foreach (RaycastHit2D hit in hits)
        {
            GridObjectBase hitGridObj = hit.collider.GetComponent<GridObjectBase>();
            if (hitGridObj.CanMove == false)
            {
                _canMove = false; break;
            }
            else if(hitGridObj.IsPushable)
            {
                _canMove = hitGridObj.MoveControl(movedir, currentMap);
            }

        }
        if (_canMove)
        {
            DidInteracted = false;
            currentMap.Add(this.gameObject, this.gameObject.transform.position);
            transform.Translate(movedir);
        }
        return _canMove;
    }
    public bool CheckMoveDir(Vector2 movedir)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + (Vector3)movedir, movedir, .1f);
        foreach (RaycastHit2D hit in hits)
        {
            GridObjectBase hitGridObj = hit.collider.GetComponent<GridObjectBase>();
            if (hitGridObj.CanMove == false)
            {
                return false;
            }
            else if (hitGridObj.IsPushable)
            {
                return hitGridObj.CheckMoveDir(movedir);
            }
        }
        return true;
    }
    public bool HasProperty(PropertyObject.Type propertyType)
    {
        if (!IsActive) return false;
        return _ruleParser.HasObjectRule(EnumType, VerbObject.Type.Is, propertyType);
    }

    
    public abstract Enum GetObjectType();
}
