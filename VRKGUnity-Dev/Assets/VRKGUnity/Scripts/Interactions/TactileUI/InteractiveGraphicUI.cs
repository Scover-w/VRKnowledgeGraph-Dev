using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace AIDEN.TactileUI
{
    [Serializable]
    public class InteractiveGraphicUI
    {
#if UNITY_EDITOR
        public Graphic Graphic { get { return _graphic; } }

        [HideInInspector]
        public string Name;
#endif

        [SerializeField]
        Graphic _graphic;

        [SerializeField]
        InteractiveColorUI _interactiveColorUI;

        public void UpdateColor(InteractionStateUI state)
        {
            switch (state)
            {
                case InteractionStateUI.Normal:
                    _graphic.color = _interactiveColorUI.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    _graphic.color = _interactiveColorUI.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    _graphic.color = _interactiveColorUI.ActivatedColor;
                    break;
                case InteractionStateUI.Disabled:
                    _graphic.color = _interactiveColorUI.DisabledColor;
                    break;
            }
        }
    }

    public static class InteractiveGraphicUIExtensions
    {
        public static void UpdateColor(this List<InteractiveGraphicUI> interactiveGraphics, InteractionStateUI interactionState)
        {
            foreach (InteractiveGraphicUI item in interactiveGraphics)
            {
                if (item.Graphic == null)
                    continue;

                item.UpdateColor(interactionState);
            }
        }

        public static void TrySetName(this List<InteractiveGraphicUI> interactiveGraphics)
        {
            foreach (InteractiveGraphicUI item in interactiveGraphics)
            {
                if (item.Graphic == null)
                    continue;

                item.Name = item.Graphic.name;
            }
        }

        public static void TrySetName(this InteractiveGraphicUI interactiveGraphic)
        {
            if (interactiveGraphic.Graphic == null)
                return;

            interactiveGraphic.Name = interactiveGraphic.Graphic.name;
        }
    }
}