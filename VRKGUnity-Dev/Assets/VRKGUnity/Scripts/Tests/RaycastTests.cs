using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastTests : MonoBehaviour
{

    public string RaycastStatus;

    [SerializeField]
    Camera _cam;

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            RaycastStatus = "PointerOverGameObject";
            return;
        }

        RaycastStatus = "";
    }
}
