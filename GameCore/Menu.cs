using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Menu
    {
        public Menu(MenuType pmType, Menu pmParentMenu)
        {
            type = pmType;
            parentMenu = pmParentMenu;
        }

        #region declaration        
        public int selectedIndex = 0;
        public MenuType type = MenuType.MenuType_Main;
        public Menu parentMenu;
        #endregion

        #region business

        #endregion
    }

    public enum MenuType
    {
        MenuType_None,
        MenuType_Main,
        MenuType_Character,
        MenuType_Item,
        MenuType_Medical,
        MenuType_Detox,
        MenuType_System
    }
}