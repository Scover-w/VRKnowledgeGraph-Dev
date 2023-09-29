using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Sphere that act as an interactor with the graph nodes (more precise than a grab hand system)
/// </summary>
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
