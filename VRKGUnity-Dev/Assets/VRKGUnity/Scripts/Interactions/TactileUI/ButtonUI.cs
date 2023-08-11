using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

