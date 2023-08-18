using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskDirectActivator : MonoBehaviour
{
    [SerializeField]
    GameObject _deskDirectInteractorGo;

    private void OnEnable()
    {
        _deskDirectInteractorGo.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.DeskGraph))
            return; 

        _deskDirectInteractorGo.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.DeskGraph))
            return;

        _deskDirectInteractorGo.SetActive(false);
    }
}
