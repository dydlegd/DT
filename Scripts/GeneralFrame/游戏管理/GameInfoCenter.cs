using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public enum GameType
    {
        Null,
        大厅,
        麻将
    }

    public class GameInfoCenter:SingletonBase<GameInfoCenter>
    {
        private Dictionary<GameType, GameInfo> mGameDictionary = new Dictionary<GameType, GameInfo>();

        public void Init()
        {
            AddAllGame();
        }

        public GameInfo GetGameInfo(GameType type)
        {
            return mGameDictionary[type];
        }

        private void AddAllGame()
        {
            AddGame(GameType.大厅, SceneName.Hall);
            AddGame(GameType.麻将, SceneName.MJ_Main);
        }

        private void AddGame(GameType type, SceneName sceneName)
        {
            GameInfo game = new GameInfo(type, sceneName);
            //game.Version_Server = 

            mGameDictionary.Add(type, game);
        }
    }
}

