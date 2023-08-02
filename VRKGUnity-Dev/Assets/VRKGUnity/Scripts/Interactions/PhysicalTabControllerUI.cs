using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalTabControllerUI : MonoBehaviour
{
    [SerializeField]
    List<ColorTab> _unselectedColorTabs;

    [SerializeField]
    List<ColorTab> _selectedColorTabs;

    [SerializeField]
    List<PhysicalTabUI> _tabsBtn;

    [SerializeField]
    PhysicalTabUI _selectedTab;

    private void OnEnable()
    {
        foreach(PhysicalTabUI tabBtn in _tabsBtn)
        {
            if(tabBtn == _selectedTab)
            {
                tabBtn.Select(_selectedColorTabs);
            }
            else
            {
                tabBtn.UnSelect(_unselectedColorTabs);
            }
        }
    }

    public void Select(PhysicalTabUI newSelected)
    {
        if (newSelected == _selectedTab)
            return;

        _selectedTab.UnSelect(_unselectedColorTabs);
        _selectedTab = newSelected;
        _selectedTab.Select(_selectedColorTabs);  
    }

    [Serializable]
    public class ColorTab
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
