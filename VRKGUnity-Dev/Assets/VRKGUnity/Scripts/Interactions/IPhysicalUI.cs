using UnityEngine;

public interface IPhysicalUI
{
    public void TriggerEnter(bool isProximity, Collider collider);
    public void TriggerExit(bool isProximity, Collider collider);
}
