using UnityEngine;



namespace AIDEN.TactileUI
{
    public interface ITactileUI
    {
        public void TriggerEnter(bool isProximity, Transform touchTf);
        public void TriggerExit(bool isProximity, Transform touchTf);
    }

}
