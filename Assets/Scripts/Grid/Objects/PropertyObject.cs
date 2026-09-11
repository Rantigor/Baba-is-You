using System;
using UnityEngine;

public class PropertyObject : GridObject<PropertyObject>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }
    public enum Type
    {
        You,
        You2,
        Push,
        Stop,
        Win,
        Defeat,
        Sink,
        Open,
        Shut,
        Move,
        Float,
        Hot,
        Melt
    }
    [SerializeField] private Type _type;

    public override Enum EnumType => _type;

    public override Enum GetObjectType()
    {
        return _type;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
