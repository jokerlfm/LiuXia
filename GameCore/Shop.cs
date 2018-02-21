using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Shop
    {
        public Shop(int pmID)
        {
            this.shopID = pmID;
            itemArray = new SellingItem[5];
        }

        #region declaration
        public int shopID = 0;
        public SellingItem[] itemArray;
        #endregion
    }

    public class SellingItem
    {
        public SellingItem()
        {

        }

        #region declaration
        public int itemID = 0;
        public int itemCount = 0;
        public int requireItemID = 0;
        public int RequireItemAmount = 0;
        #endregion
    }
}