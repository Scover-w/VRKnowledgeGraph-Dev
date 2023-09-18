using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OfflineEvent : MonoBehaviour
{
    [SerializeField]
    UnityEvent _isOffline;

    [SerializeField]
    UnityEvent _isOnline;


    private void OnEnable()
    {
        if(Settings.IS_MODE_IN_OFFLINE)
            _isOffline?.Invoke();
        else
            _isOnline?.Invoke();
    }


}
