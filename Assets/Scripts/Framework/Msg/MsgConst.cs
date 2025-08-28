namespace KidGame.Core
{
    /// <summary>
    /// ��Ϣ������ id��Ҫ�ظ�������
    /// </summary>
    public class MsgConst
    {

        #region ������� 10��ͷ

        //��Ϸ�ڲ��� 100��ͷ
        public const int ON_INTERACTION_PRESS = 10001; //���� - ����������
        public const int ON_DASH_PRESS = 10002; //���� - ��̼�����
        public const int ON_RUN_PRESS = 10003; //���� - ���ܼ�����
        public const int ON_RUN_RELEASE = 10004; //���� - ���ܼ�̧��
        public const int ON_USE_PRESS = 10005; //���� - ʹ�ü�����
        public const int ON_USE_LONG_PRESS = 10006; //���� - ʹ�ü�����
        public const int ON_USE_LONG_PRESS_RELEASE = 10007; //���� - ʹ�ü�����̧��
        public const int ON_BAG_PRESS = 10008; //���� - ����������
        public const int ON_PICK_PRESS = 10009; //���� - ���ռ�����
        public const int ON_MOUSEWHEEL_VALUE_CHG = 10010; //���� - ������ֵ�ı�
        public const int ON_GAMEPAUSE_PRESS = 10011; //���� - ��ͣ������
        public const int ON_MOUSEWHEEL_PRESS = 10012; //���� - �����ּ�����

        //UI���� 101��ͷ
        public const int ON_UI_INTERACTION_PRESS = 10101; //���� - ����������
        public const int ON_UI_BAG_PRESS = 10102; //���� - ����������

        #endregion
        
        #region ��ҹ������ 20��ͷ

        public const int ON_SELECT_ITEM = 20001; //��� - ѡ���������Ʒ
        public const int ON_QUICK_BAG_UPDATE = 20002; //��� - ������ˢ��
        public const int ON_STAMINA_CHG = 20003; //��� - �����ı�
        public const int ON_HEALTH_CHG = 20004; //��� - �����ı�
        public const int ON_PLAYER_DEAD = 20005; //��� - �������
        public const int ON_PICK_ITEM = 20006; //��� - ������Ʒ

        #endregion

        #region ��Ϸ������� 30��ͷ

        public const int ON_GAME_START = 30001;//��Ϸ���� - �ܵ���Ϸ��ʼ
        public const int ON_GAME_FINISH = 30002;//��Ϸ���� - �ܵ���Ϸ����
        public const int ON_CUR_LOOP_SCORE_CHG = 30003;//��Ϸ���� - ����ķ����ı�
        public const int ON_PHASE_TIME_UPDATE = 30004;//��Ϸ���� - ��ҹʱ��仯
        public const int ON_LEVEL_START = 30005;//��Ϸ���� - һ�쿪ʼ
        public const int ON_LEVEL_FINISH = 30006;//��Ϸ���� - һ�����

        #endregion
        
        #region ������� 40��ͷ

        public const int ON_ENEMY_SANITY_CHG = 40001;    // ���� - ����ֵ�仯
        public const int ON_ENEMY_BUFF_CHG   = 40002;    // ���� - Buff�仯
        public const int ON_ENEMY_DIZZY      = 40003;    // ���� - ѣ��״̬

        #endregion

        #region ϵͳ��� 99��ͷ
        public const int ON_CONTROL_MAP_CHG = 99001;//ϵͳ - ����ӳ�䷽ʽ�л� Game/UI
        public const int ON_CONTROL_TYPE_CHG = 99002;//ϵͳ - ���뷽ʽ�л� ����/�ֱ�
        public const int ON_LANGUAGE_CHG = 99003;//ϵͳ - �����л�
        #endregion
    }
}
