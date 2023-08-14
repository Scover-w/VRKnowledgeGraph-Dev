using UnityEngine;

namespace AIDEN.TactileUI
{
    public interface ITouchUI
    {
        public bool Interactable { get; set; }

        public void TriggerEnter(bool isProximity, Transform touchTf);
        public void TriggerExit(bool isProximity, Transform touchTf);
    }

    public interface IValueUI<T>
    {
        public T Value { get; set; }
    }

}
