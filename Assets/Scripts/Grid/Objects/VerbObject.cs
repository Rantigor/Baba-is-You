using System;
using UnityEngine;

public class VerbObject : GridObject<VerbObject>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }
    public enum Type
    {
        Is,
        Has,
        Make
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
