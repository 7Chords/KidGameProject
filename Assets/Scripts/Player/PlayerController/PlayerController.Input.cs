using UnityEngine;
using Utils;

namespace KidGame.Core
{
    public partial class PlayerController
    {
        /// <summary>
        /// 注册事件们
        /// </summary>
        private void RegActions()
        {
            MsgCenter.RegisterMsgAct(MsgConst.ON_INTERACTION_PRESS, PlayerInteraction);
            MsgCenter.RegisterMsgAct(MsgConst.ON_PICK_PRESS, PlayerPick);
            MsgCenter.RegisterMsgAct(MsgConst.ON_USE_PRESS, PlayerUseItem);
            MsgCenter.RegisterMsgAct(MsgConst.ON_BAG_PRESS, ControlBag);
            MsgCenter.RegisterMsgAct(MsgConst.ON_GAMEPAUSE_PRESS, GamePause);
            MsgCenter.RegisterMsgAct(MsgConst.ON_USE_LONG_PRESS, TryUseWeaponUseLongPress);
            MsgCenter.RegisterMsg(MsgConst.ON_SELECT_ITEM, OnItemSelected);
            MsgCenter.RegisterMsg(MsgConst.ON_PLAYER_HIDE_CHG, OnPlayerUnderTableChg);
        }

        /// <summary>
        /// 反注册事件们
        /// </summary>
        private void UnregActions()
        {
            MsgCenter.UnregisterMsgAct(MsgConst.ON_INTERACTION_PRESS, PlayerInteraction);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_PICK_PRESS, PlayerPick);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_USE_PRESS, PlayerUseItem);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_BAG_PRESS, ControlBag);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_GAMEPAUSE_PRESS, GamePause);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_USE_LONG_PRESS, TryUseWeaponUseLongPress);
            MsgCenter.UnregisterMsg(MsgConst.ON_SELECT_ITEM, OnItemSelected);
            MsgCenter.UnregisterMsg(MsgConst.ON_PLAYER_HIDE_CHG, OnPlayerUnderTableChg);
        }

        private void GamePause()
        {
            Signals.Get<OpenPauseSignal>().Dispatch();
        }

        private void ControlBag()
        {
            Signals.Get<ControlBackpackPanelSignal>().Dispatch();
        }

        private void OnPlayerUnderTableChg(object[] objs)
        {
            if (objs == null || objs.Length == 0) return;
            playerInfo.IsHide = (bool)objs[0];
        }
    }
}