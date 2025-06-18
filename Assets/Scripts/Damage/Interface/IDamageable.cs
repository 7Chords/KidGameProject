using KidGame.Core;

namespace KidGame.Interface
{
    /// <summary>
    /// 可受伤的接口
    /// </summary>
    public interface IDamageable
    {
        public abstract void TakeDamage(DamageInfo damageInfo);
    }
}
