using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentInheTest : MonoBehaviour
{
    public virtual void Call()
    {
        Debug.Log("Call Parent");

        Call2();
    }

    public virtual void Call2()
    {
        Debug.Log("Call2 Parent");
    }
}
