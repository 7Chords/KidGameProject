using KidGame.Core;
using KidGame.Core.Data;

namespace KidGame.Core
{
    public class GameModel : SingletonNoMono<GameModel>
    {
        private PlayerInfo playerInfo;

        private GameLevelInfo gameLevelInfo;

        private PlayerBagInfo playerBagInfo;

        public PlayerInfo PlayerInfo => playerInfo;
        public GameLevelInfo GameLevelInfo => gameLevelInfo;
        public PlayerBagInfo PlayerBagInfo => playerBagInfo;

        #region 初始化方法
        
        public void Init(PlayerBaseData playerData)
        {
            playerInfo = new PlayerInfo();
            playerInfo.Init(playerData);

            gameLevelInfo = new GameLevelInfo();
            gameLevelInfo.Init();

            playerBagInfo = new PlayerBagInfo();
            playerBagInfo.Init();
        }

        public void LoadPlayerInfo(PlayerInfo _playerInfo)
        {
            playerInfo = _playerInfo;
        }

        public void LoadGameLevelInfo(GameLevelInfo _gameLevelInfo)
        {
            gameLevelInfo = _gameLevelInfo;
        }

        public void LoadPlayerBagInfo(PlayerBagInfo _playerBagInfo)
        {
            playerBagInfo = _playerBagInfo;
        }
        #endregion
    }

}
