using KidGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.core
{
    public class KGmodel : SingletonNoMono<KGmodel>
    {
        #region 信息字段

        PlayerInfo playerInfo;
        GameLevelInfo gameLevelInfo;
        PlayerBagInfo playerBagInfo;
        #endregion

        #region 属性字段
        public PlayerInfo PlayerInfo => playerInfo;
        public GameLevelInfo GameLevelInfo => gameLevelInfo;

        public PlayerBagInfo PlayerBagInfo => playerBagInfo;
        #endregion

        #region 初始化方法
        public void InitialPlayerInfo(PlayerInfo _playerInfo)
        {
            playerInfo = _playerInfo;
        }

        public void InitialGameLevelInfo(GameLevelInfo _gameLevelInfo)
        {
            gameLevelInfo = _gameLevelInfo;
        }

        public void InitialPlayerBagInfo(PlayerBagInfo _playerBagInfo)
        {
            playerBagInfo = _playerBagInfo;
        }
        #endregion
    }

}
