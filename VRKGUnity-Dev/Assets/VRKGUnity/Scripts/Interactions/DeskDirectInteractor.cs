using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskDirectInteractor : MonoBehaviour
{
    [SerializeField]
    RayActivator _rayInteractor;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.Node))
            return;

        _rayInteractor.SetDeskInteractorWantToHideIt(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.Node))
            return;

        _rayInteractor.SetDeskInteractorWantToHideIt(false);
    }


    private void OnDisable()
    {
        _rayInteractor.SetDeskInteractorWantToHideIt(false);
    }
}
