namespace KidGame.Core
{
    /// <summary>
    /// 消息常量类 id不要重复！！！
    /// </summary>
    public class MsgConst
    {

        #region 输入相关 10开头

        public const int ON_INTERACTION_PRESS = 10001;
        public const int ON_DASH_PRESS = 10002;
        public const int ON_RUN_PRESS = 10003;
        public const int ON_RUN_RELEASE = 10004;
        public const int ON_USE_PRESS = 10005;
        public const int ON_USE_LONG_PRESS = 10006;
        public const int ON_USE_LONG_PRESS_RELEASE = 10007;
        public const int ON_BAG_PRESS = 10008;
        public const int ON_PICK_PRESS = 10009;
        public const int ON_MOUSEWHEEL_VALUE_CHG = 10010;
        public const int ON_GAMEPAUSE_PRESS = 10011;
        public const int ON_MOUSEWHEEL_PRESS = 10012;
        public const int ON_INTERACTION_PRESS_WITHOUT_TIME = 10013;


        #endregion


        #region 玩家功能相关 20开头

        public const int ON_SELECT_ITEM = 20001;//玩家 - 选择道具栏物品
        public const int ON_QUICK_BAG_UPDATE = 20002;//玩家 - 道具栏刷新
        public const int ON_STAMINA_CHG = 20003;//玩家 - 体力改变
        public const int ON_HEALTH_CHG = 20004;//玩家 - 生命改变
        public const int ON_PLAYER_DEAD = 20005;//玩家 - 玩家死亡
        public const int ON_PICK_ITEM = 20006;//玩家 - 回收物品

        #endregion

        #region 游戏流程相关 30开头

        public const int ON_GAME_START = 30001;//游戏流程 - 总的游戏开始
        public const int ON_GAME_FINISH = 30002;//游戏流程 - 总的游戏结束
        public const int ON_CUR_LOOP_SCORE_CHG = 30003;//游戏流程 - 当天的分数改变
        public const int ON_PHASE_TIME_UPDATE = 30004;//游戏流程 - 昼夜时间变化
        #endregion
    }
}
