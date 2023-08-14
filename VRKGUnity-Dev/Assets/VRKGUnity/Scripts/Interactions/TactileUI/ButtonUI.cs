using UnityEngine;
using UnityEngine.Events;

namespace AIDEN.TactileUI
{
    public class ButtonUI : BaseTouch
    {
        [SerializeField, Space(10)]
        UnityEvent _onClick;

        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            base.Activate();


            _touchInter.ActivateHaptic();
            _onClick?.Invoke();
        }
    }

}

