namespace KidGame.Core
{
    /// <summary>
    /// ��������״̬����ȫ��״̬
    /// </summary>
    public enum EnemyState
    {
        Idle,
        Patrol,
        SearchTargetRoom,   // ��Ŀ�ĵ��ѷ���
        CheckNoiseSource,   // �����Դ������������
        Chase,              // ׷����ң������
        Attack
    }
}