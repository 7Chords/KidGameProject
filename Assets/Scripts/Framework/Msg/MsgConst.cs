namespace KidGame.Core
{
    /// <summary>
    /// 消息常量类 id不要重复！！！
    /// </summary>
    public class MsgConst
    {

        #region 输入相关 10开头

        //游戏内操作 100开头
        public const int ON_INTERACTION_PRESS = 10001; //输入 - 交互键按下
        public const int ON_DASH_PRESS = 10002; //输入 - 冲刺键按下
        public const int ON_RUN_PRESS = 10003; //输入 - 奔跑键按下
        public const int ON_RUN_RELEASE = 10004; //输入 - 奔跑键抬起
        public const int ON_USE_PRESS = 10005; //输入 - 使用键按下
        public const int ON_USE_LONG_PRESS = 10006; //输入 - 使用键长按
        public const int ON_USE_LONG_PRESS_RELEASE = 10007; //输入 - 使用键长按抬起
        public const int ON_BAG_PRESS = 10008; //输入 - 背包键按下
        public const int ON_PICK_PRESS = 10009; //输入 - 回收键按下
        public const int ON_MOUSEWHEEL_VALUE_CHG = 10010; //输入 - 鼠标滚轮值改变
        public const int ON_GAMEPAUSE_PRESS = 10011; //输入 - 暂停键按下
        public const int ON_MOUSEWHEEL_PRESS = 10012; //输入 - 鼠标滚轮键按下

        //UI操作 101开头
        public const int ON_UI_INTERACTION_PRESS = 10101; //输入 - 交互键按下
        public const int ON_UI_BAG_PRESS = 10102; //输入 - 背包键按下

        #endregion
        
        #region 玩家功能相关 20开头

        public const int ON_SELECT_ITEM = 20001; //玩家 - 选择道具栏物品
        public const int ON_QUICK_BAG_UPDATE = 20002; //玩家 - 道具栏刷新
        public const int ON_STAMINA_CHG = 20003; //玩家 - 体力改变
        public const int ON_HEALTH_CHG = 20004; //玩家 - 生命改变
        public const int ON_PLAYER_DEAD = 20005; //玩家 - 玩家死亡
        public const int ON_PICK_ITEM = 20006; //玩家 - 回收物品

        #endregion

        #region 游戏流程相关 30开头

        public const int ON_GAME_START = 30001;//游戏流程 - 总的游戏开始
        public const int ON_GAME_FINISH = 30002;//游戏流程 - 总的游戏结束
        public const int ON_CUR_LOOP_SCORE_CHG = 30003;//游戏流程 - 当天的分数改变
        public const int ON_PHASE_TIME_UPDATE = 30004;//游戏流程 - 昼夜时间变化
        public const int ON_LEVEL_START = 30005;//游戏流程 - 一天开始
        public const int ON_LEVEL_FINISH = 30006;//游戏流程 - 一天结束

        #endregion
        
        #region 敌人相关 40开头

        public const int ON_ENEMY_SANITY_CHG = 40001;    // 敌人 - 理智值变化
        public const int ON_ENEMY_BUFF_CHG   = 40002;    // 敌人 - Buff变化
        public const int ON_ENEMY_DIZZY      = 40003;    // 敌人 - 眩晕状态

        #endregion

        #region 系统相关 99开头
        public const int ON_CONTROL_MAP_CHG = 99001;//系统 - 输入映射方式切换 Game/UI
        public const int ON_CONTROL_TYPE_CHG = 99002;//系统 - 输入方式切换 键盘/手柄
        public const int ON_LANGUAGE_CHG = 99003;//系统 - 语言切换
        #endregion
    }
}
