using KidGame.Core;

namespace KidGame.Interface
{
    /// <summary>
    /// �����˵Ľӿ�
    /// </summary>
    public interface IDamageable
    {
        public abstract void TakeDamage(DamageInfo damageInfo);
    }
}
