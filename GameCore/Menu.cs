using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Menu
    {
        public Menu(MenuType pmType, string pmMenuName, Menu pmParentMenu, bool pmDrawParents = false)
        {
            type = pmType;
            this.menuName = pmMenuName;
            parentMenu = pmParentMenu;
            this.subMenuList = new List<Menu>();
            drawParents = pmDrawParents;
        }

        #region declaration        
        public int subSelectedIndex = 0;
        public MenuType type = MenuType.MenuType_Main;
        public Menu parentMenu;
        public List<Menu> subMenuList;
        public string menuName = "";
        public int contexID = 0;
        public bool drawParents = false;
        #endregion

        #region business

        #endregion
    }

    public enum MenuType
    {
        MenuType_None,
        MenuType_Main,

        MenuType_Medical,
        MenuType_Detox,
        MenuType_Item,
        MenuType_Item_ChooseUser,
        MenuType_Item_UseResult,

        MenuType_Status,

        MenuType_Leave,
        MenuType_System,

        MenuType_Save,
        MenuType_Load,
        MenuType_Full,
        MenuType_Quit,
        MenuType_Quit_Yes,
        MenuType_Quit_No,
    }
}