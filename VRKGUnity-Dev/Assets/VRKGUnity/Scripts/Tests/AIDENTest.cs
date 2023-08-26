using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDENTest : MonoBehaviour
{
    [SerializeField]
    AIDENController _aiden;


    public string UserSentence;

    [ContextMenu("Ask")]
    public void Ask()
    {
        _aiden.GenerateResponse(UserSentence);
    }

}
