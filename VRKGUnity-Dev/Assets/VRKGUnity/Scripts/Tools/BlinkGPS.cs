using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkGPS : MonoBehaviour
{
    [SerializeField]
    MeshRenderer _renderer;

    bool _isDisplayed = true;



    // Update is called once per frame
    void Update()
    {
        float time = Time.time % 2f;

        if(time < 1.5f && !_isDisplayed)
        {
            _isDisplayed = true;
            _renderer.enabled = true;
        }
        else if(time > 1.5f && _isDisplayed)
        {
            _isDisplayed = false;
            _renderer.enabled = false;
        }

    }
}
