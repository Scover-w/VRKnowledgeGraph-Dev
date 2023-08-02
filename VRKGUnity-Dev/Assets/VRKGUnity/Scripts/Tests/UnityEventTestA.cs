using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventTestA : MonoBehaviour
{
    [SerializeField]
    UnityEvent _onClick;

    [SerializeField]
    UnityEvent<char> _onWrite;

    public char Value;

    [ContextMenu("OnClick")]
    public void OnClick()
    {
        _onClick?.Invoke();
    }

    [ContextMenu("OnWrite")]
    public void OnWriteBoy()
    {
        _onWrite?.Invoke(Value);
    }

}
