using System;
using UnityEngine;

public class ModifierObject : GridObject<ModifierObject>
{
    public override Enum EnumType => throw new NotImplementedException();

    public override Enum GetObjectType()
    {
        throw new NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
