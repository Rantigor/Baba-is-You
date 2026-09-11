using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class GridObject<T> : GridObjectBase where T : GridObject<T>
{
    protected override void Start()
    {
        base.Start();
    }
    
}
