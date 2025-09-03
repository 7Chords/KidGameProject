using KidGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.core
{
    public class PlayerBagInfo
    {
        private List<ISlotInfo> quickAccessBag = new List<ISlotInfo>();//道具栏，最多4个
        private List<ISlotInfo> backBag = new List<ISlotInfo>();//库存背包
        private int _selectedIndex = 0;
        public List<ISlotInfo> QuickAccessBag
        {
            get => quickAccessBag;
            set => quickAccessBag = value;
        }

        public List<ISlotInfo> BackBag
        {
            get => backBag;
            set => backBag = value;
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => _selectedIndex = value;
        }
    }

}
