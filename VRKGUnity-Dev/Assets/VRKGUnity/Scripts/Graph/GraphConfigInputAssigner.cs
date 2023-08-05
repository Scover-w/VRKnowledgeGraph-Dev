using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphConfigInputAssigner : MonoBehaviour
{
    [SerializeField]
    GraphConfigurationKey _graphConfigurationKey;

    [SerializeField]
    MonoBehaviour _tactileUIScript;

    private void OnEnable()
    {
        //_slider.value = 6f;
    }

    private void OnValidate()
    {
        if (_tactileUIScript == null)
            return;


    }
}
