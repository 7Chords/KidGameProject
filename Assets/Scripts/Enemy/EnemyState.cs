namespace KidGame.Core
{
    /// <summary>
    /// 敌人有限状态机的全部状态
    /// </summary>
    public enum EnemyState
    {
        Idle,
        Patrol,
        SearchTargetRoom,   // 有目的地搜房间
        CheckNoiseSource,   // 检查声源（翻床、翻柜）
        Chase,              // 追击玩家，含冲刺
        Attack
    }
}