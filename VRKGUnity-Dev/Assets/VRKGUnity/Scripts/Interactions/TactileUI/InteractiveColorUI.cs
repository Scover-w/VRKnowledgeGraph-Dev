using System;
using UnityEngine;

namespace AIDEN.TactileUI
{
    [Serializable]
    public class InteractiveColorUI
    {
        public Color NormalColor { get { return _normalColor; } }
        public Color ProximityColor { get { return _proximityColor; } }
        public Color ActivatedColor { get { return _activatedColor; } }
        public Color DisabledColor { get { return _disabledColor; } }


        [SerializeField]
        Color _normalColor;

        [SerializeField]
        Color _proximityColor;

        [SerializeField]
        Color _activatedColor;

        [SerializeField]
        Color _disabledColor;
    }
}