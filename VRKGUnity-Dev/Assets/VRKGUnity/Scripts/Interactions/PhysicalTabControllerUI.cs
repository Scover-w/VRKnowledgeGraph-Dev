using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PhysicalTabControllerUI : MonoBehaviour
{
    [SerializeField]
    List<ColorStateUI> _unselectedColorStates;

    [SerializeField]
    List<ColorStateUI> _selectedColorStates;

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
                tabBtn.Select(_selectedColorStates);
            }
            else
            {
                tabBtn.UnSelect(_unselectedColorStates);
            }
        }
    }

    public void Select(PhysicalTabUI newSelected)
    {
        if (newSelected == _selectedTab)
            return;

        _selectedTab.UnSelect(_unselectedColorStates);
        _selectedTab = newSelected;
        _selectedTab.Select(_selectedColorStates);  
    }
}
