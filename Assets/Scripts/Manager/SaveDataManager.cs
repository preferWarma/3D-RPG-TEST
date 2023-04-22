using Inventory.Logic;
using Quest.Logic;
using Transition;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Manager
{
    public class SaveDataManager : Singleton<SaveDataManager>
    {
        private string _currentScene = "currentScene";

        public string SceneName => PlayerPrefs.GetString(_currentScene);
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))   // 暂停回到主菜单
            {
                SceneController.Instance.TransitionToMenu();
            }
            
            if (Input.GetKeyDown(KeyCode.Z))    // 按B保存数据
            {
                Instance.SavePlayerData();  // 保存数据
                InventoryManager.Instance.SaveData();   // 保存背包数据
                QuestManager.Instance.SaveQuest();  // 保存任务数据
            }

            if (Input.GetKeyDown(KeyCode.L)) // 按L加载数据
            {
                Instance.LoadPlayerData();
                InventoryManager.Instance.LoadData();
                QuestManager.Instance.LoadQuest();
            }
        }

        public void SavePlayerData()    // 保存玩家数据
        {
            var playerData = GameManager.Instance.playerStats.characterData;
            SaveData(playerData, playerData.name);
        }
        
        public void LoadPlayerData()    // 加载玩家数据
        {
            var playerData = GameManager.Instance.playerStats.characterData;
            LoadData(playerData, playerData.name);
        }
        
        public void SaveData<T>(T obj, string key)
        {
            var jsonData = JsonUtility.ToJson(obj, true);
            PlayerPrefs.SetString(key, jsonData);   // 保存数据
            PlayerPrefs.SetString(_currentScene, SceneManager.GetActiveScene().name);   // 保存场景名字
            PlayerPrefs.Save();
        }

        public void LoadData<T>(T obj, string key)
        {
            if (!PlayerPrefs.HasKey(key)) return;
            var jsonData = PlayerPrefs.GetString(key);
            JsonUtility.FromJsonOverwrite(jsonData, obj);
        }
        
    }// class SaveDataManager
    
}// namespace Manager
