using UnityEngine;



namespace AIDEN.TactileUI
{
    public interface ITactileUI
    {
        public void TriggerEnter(bool isProximity, Collider collider);
        public void TriggerExit(bool isProximity, Collider collider);
    }

}
