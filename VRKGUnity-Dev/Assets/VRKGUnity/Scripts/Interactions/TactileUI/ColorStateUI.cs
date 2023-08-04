using System;
using UnityEngine;

namespace AIDEN.TactileUI
{
    [Serializable]
    public class ColorStateUI
    {
        public Color NormalColor { get { return _normalColor; } }
        public Color ProximityColor { get { return _proximityColor; } }
        public Color ActivatedColor { get { return _activatedColor; } }


        [SerializeField]
        Color _normalColor;

        [SerializeField]
        Color _proximityColor;

        [SerializeField]
        Color _activatedColor;
    }
}