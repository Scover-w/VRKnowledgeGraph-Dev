using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class KeyUI : BaseTouch, IValueUI<char>
    {
        public char Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _label.text = value.ToString();
            }
        }


        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        char _value;

        [SerializeField]
        UnityEvent<char> _onKey;

        protected override void OnEnable()
        {
            base.OnEnable();
 
            ToLower();
        }

        public void ToLower()
        {
            _label.fontStyle = FontStyles.LowerCase;
            _value = char.ToLower(_value);
        }

        public void ToUpper()
        {
            _label.fontStyle = FontStyles.UpperCase;
            _value = char.ToUpper(_value);
        }

        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            base.Activate();

            _touchInter.ActivateHaptic();
            _onKey?.Invoke(_value);
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _label.text = _value.ToString();
        }
#endif
    }
}