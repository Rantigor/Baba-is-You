using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


public class PlayerMovement : MonoBehaviour
{
    private Vector2 moveDir = Vector2.zero;
    private bool canMove = true;
    private Keyboard _keyboard;
    private float delayTime = .15f;
    private RuleParser _ruleParser;
    private UndoSystem _undoSystem;

    private void Awake()
    {
        _keyboard = Keyboard.current;
        _undoSystem = FindAnyObjectByType<UndoSystem>();
    }

    void Start()
    {
        _ruleParser = FindAnyObjectByType<RuleParser>();
        _ruleParser.RuleParse();
    }

    // Update is called once per frame
    void Update()
    {
        InputListen();
        Move();
    }
    private void InputListen()
    {
        if (_keyboard.wKey.isPressed)
        {
            moveDir.y = 1;
        }
        else if (_keyboard.sKey.isPressed)
        {
            moveDir.y = -1;
        }
        else { moveDir.y = 0; }

        if(_keyboard.dKey.isPressed)
        {
            moveDir.x = 1;
        }
        else if( _keyboard.aKey.isPressed)
        {
            moveDir.x = -1;
        }
        else { moveDir.x = 0; }
    }
    private void Move()
    {
        if(delayTime < 0)
        {
            if(_keyboard.zKey.isPressed)
            {
                _undoSystem.Undo();
                delayTime = .15f;
                return;
            }
            if (moveDir == Vector2.zero) return;
            Dictionary<GameObject, Vector2> currentMoveMap = new Dictionary<GameObject, Vector2>();
            foreach (GameObject obj in _ruleParser.YouObjects)
            {
                if (!obj.GetComponent<GridObjectBase>().IsActive) return;
                RaycastHit2D[] hits = Physics2D.RaycastAll(obj.transform.position + (Vector3)moveDir ,moveDir, .1f);
                canMove = true;

                foreach(RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject == obj.gameObject) continue;
                    GridObjectBase hitGridObj = hit.collider.GetComponent<GridObjectBase>();
                    if (hitGridObj.CanMove == false)
                    {
                        canMove = false; break;
                    }
                    if (hitGridObj.IsPushable)
                    {
                        canMove = hitGridObj.CheckMoveDir(moveDir);
                    }

                }

                if(canMove)
                {
                    foreach(RaycastHit2D hit in hits)
                    {
                        GridObjectBase hitGridObj = hit.collider.GetComponent<GridObjectBase>();
                        if (hitGridObj.IsPushable)
                        {
                            hitGridObj.MoveControl(moveDir, currentMoveMap);
                        }
                        else
                        {
                            currentMoveMap.Add(hitGridObj.gameObject, hitGridObj.transform.position);
                        }
                    }
                    currentMoveMap.Add(obj.gameObject, obj.transform.position);
                    obj.transform.Translate(moveDir);
                    Physics2D.SyncTransforms();
                    delayTime = .15f;
                }
            }
            if (currentMoveMap.Count != 0)
            { _undoSystem.OldMaps.Push(new Dictionary<GameObject,Vector2>(currentMoveMap)); }


            _ruleParser.RuleParse();
        }
        else
        {
            delayTime -= Time.deltaTime;
        }
    }
}
