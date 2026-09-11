using UnityEngine;
using System;
public class NounObject : GridObject<NounObject>
{
    protected override void Start()
    {
        base.Start();
    }
    public enum Type
    {
        Baba,
        Rock,
        Wall,
        Flag,
        Water,
        Lava,
        Skull,
        Key,
        Door,
    }
    [SerializeField]private Type _type;
    public override Enum EnumType => _type;
    public override Enum GetObjectType()
    {
        return _type;
    }
    void Update()
    {
        
    }
}
