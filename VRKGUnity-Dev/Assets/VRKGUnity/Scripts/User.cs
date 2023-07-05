using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;
    
    GraphMode _graphMode = GraphMode.Desk;


    [ContextMenu("Switch Mode")]
    public void SwitchMode()
    {
        if (_graphManager.IsRunningSimulation)
        {
            Debug.Log("Can't switch Mode when running simulation");
            // TODO : Notification can't switch mode in simulation
            return;
        }

        if(_graphMode == GraphMode.Desk)
        {
            Debug.Log("Switch to Immersion Mode");
            _graphMode = GraphMode.Immersion;
            _graphManager.TrySwitchModeToImmersion();
        }
        else
        {
            Debug.Log("Switch to Desk Mode");
            _graphMode = GraphMode.Desk;
            _graphManager.TrySwitchModeToDesk();
        }
    }
}
