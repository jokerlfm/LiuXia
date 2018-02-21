using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Menu
    {
        public Menu()
        {

        }

        #region declaration
        public Menu[] subMenus;
        public Menu parentMenu;

        public int selectedIndex = 0;
        #endregion

        #region business

        #endregion
    }

    public enum MenuType
    {
        MenuType_Main,
        MenuType_Character,
        MenuType_Item,
        MenuType_Medical,
        MenuType_Detox,
        MenuType_System
    }
}