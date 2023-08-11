using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildInheTest : ParentInheTest
{
    [ContextMenu("Call")]
    public override void Call()
    {
        Debug.Log("Call Child");

        base.Call();
    }

    public override void Call2() 
    {
        Debug.Log("Call2 Child");
        base.Call2();
    }
}
