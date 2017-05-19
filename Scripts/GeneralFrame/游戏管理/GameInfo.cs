using UnityEngine;
using System.Collections;

namespace DYD
{
    /// <summary>
    /// 游戏信息
    /// </summary>
    public class GameInfo
    {
        private string strVersionKey { get { return "Version_" + type; } }
        public GameType type { get; private set; }
        public SceneName mSceneName { get; private set; }
        private int mVersionClient;
        public int Version_Client
        {
            get 
            {
                mVersionClient = PlayerPrefs.GetInt(strVersionKey, 0);
                return mVersionClient;
            }
            private set
            {
                mVersionClient = value;
                PlayerPrefs.SetInt(strVersionKey, value);
            }
        }

        public int Version_Server { get; set; }
        public bool IsNeedUpdate { get { return Version_Client < Version_Server; } }


        public GameInfo(GameType _type, SceneName _sceneName)
        {
            type = _type;
            mSceneName = _sceneName;
        }

        /// <summary>
        /// 更新版本号
        /// </summary>
        public void UpdateVerion()
        {
            Version_Client = Version_Server;
        }
    }
}

