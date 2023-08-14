using System.Collections.Generic;
using UnityEngine;

namespace AIDEN.TactileUI
{
    public partial class TabControllerUI : MonoBehaviour
    {
        [SerializeField]
        List<InteractiveColorUI> _unselectedColorStates;

        [SerializeField]
        List<InteractiveColorUI> _selectedColorStates;

        [SerializeField]
        List<TabUI> _tabsBtn;

        [SerializeField]
        TabUI _selectedTab;


        private void OnEnable()
        {
            foreach (TabUI tabBtn in _tabsBtn)
            {
                if (tabBtn == _selectedTab)
                {
                    tabBtn.Select(_selectedColorStates);
                }
                else
                {
                    tabBtn.UnSelect(_unselectedColorStates);
                }
            }
        }

        public void Select(TabUI newSelected)
        {
            if (newSelected == _selectedTab)
                return;

            _selectedTab.UnSelect(_unselectedColorStates);
            _selectedTab = newSelected;
            _selectedTab.Select(_selectedColorStates);
        }

        private void OnValidate()
        {
            foreach(TabUI tabBtn in _tabsBtn)
            {
                if (tabBtn == _selectedTab)
                    tabBtn.Select(_selectedColorStates);
                else
                    tabBtn.UnSelect(_unselectedColorStates);
            }
        }
    }
}