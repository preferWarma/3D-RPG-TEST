using System;
using System.Collections;
using System.Linq;
using Inventory.Logic;
using Manager;
using Quest.Logic;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Utils;
using Utils.Extension;


namespace Transition
{
    public class SceneController : Singleton<SceneController>, IEndGameObserver
    {
        [Label("玩家预制件")] public GameObject playerPrefab;
        [Label("渐入渐出控制件")] public SceneFaderUI canvasGroupPrefab;

        private GameObject _player;
        private NavMeshAgent _playerAgent;
        private bool _isFinished;   // 是否播放完成死亡后回到主界面的动画, 避免一直调用

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);    //加载场景的时候不会被销毁
        }

        private void OnEnable()
        {
            GameManager.Instance.AddObserver(this);
        }

        private void OnDisable()
        {
            if (!GameManager.IsInitialized) return;   // 避免Instance在OnDestroy中提前销毁导致空引用报错
            GameManager.Instance.RemoveObserver(this);
        }

        public void TransitionToDestination(TransitionPoint transitionPoint)   // 传送, 参数是当前的传送门
        {
            switch (transitionPoint.myTransitionType) // 根据传送门类型
            {
                case Enums.TransitionType.SameScene:    // 同场景
                    StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                    break;
                case Enums.TransitionType.DifferentScene: // 异场景
                    StartCoroutine(Transition(transitionPoint.destinationSceneName, transitionPoint.destinationTag));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DoingWhileSameTransition(Enums.DestinationTag destinationTag) // 同场景传送
        {
            _player = GameManager.Instance.playerStats.gameObject; // 获取玩家对象
            var destinationTransform = GetDestination(destinationTag).transform; // 得到目标位置
            _playerAgent = GameManager.Instance.playerAgent; // 玩家的导航控制器
            _playerAgent.enabled = false; // 传送时关闭移动
            _player.transform.SetPositionAndRotation(destinationTransform.position,
                destinationTransform.rotation); // 修改玩家位置和方向
            _playerAgent.enabled = true; // 传送后恢复移动
        }

        private IEnumerator Transition(string destinationSceneName, Enums.DestinationTag destinationTag)   // 协程: 传送
        {
            SaveDataManager.Instance.SavePlayerData();  // 保存数据
            InventoryManager.Instance.SaveData();   // 保存背包数据
            QuestManager.Instance.SaveQuest();  // 保存任务数据

            if (SceneManager.GetActiveScene().name == destinationSceneName)   // 同场景
            {
                DoingWhileSameTransition(destinationTag);
                yield return null;
            }
            else // 异场景
            {
                yield return SceneManager.LoadSceneAsync(destinationSceneName); // 异步加载场景
                var destinationTransform = GetDestination(destinationTag).transform;
                yield return Instantiate(playerPrefab, destinationTransform.position, destinationTransform.rotation);    // 异步加载玩家
                SaveDataManager.Instance.LoadPlayerData();  // 读取数据
            }
        }

        private TransitionDestination GetDestination(Enums.DestinationTag destinationTag)   // 获取Tag匹配的对象
        {
            var entrances = FindObjectsOfType<TransitionDestination>(); // 获取当前场景所有的传送门的数组
            return entrances.FirstOrDefault(destination => destination.myTag == destinationTag);    // 返回匹配
        }


        private IEnumerator MenuToScene(string scene)   // 从主菜单进入游戏中
        {
            if (scene == "") yield break;
            var fade = Instantiate(canvasGroupPrefab);  // 生成渐入渐出组件
            yield return StartCoroutine(fade.FadeOut());    // 渐出效果
            
            yield return SceneManager.LoadSceneAsync(scene);    // 加载场景
            var desTag = scene == "Game" ? Enums.DestinationTag.Enter : Enums.DestinationTag.D; // 目标传送门标签
            var destinationTransform = GetDestination(desTag).transform;    // 传送目标的位置
            
            yield return _player = Instantiate(playerPrefab, destinationTransform.position, destinationTransform.rotation);   // 生成玩家
            SaveDataManager.Instance.SavePlayerData();  // 保存数据
            InventoryManager.Instance.SaveData();   // 保存背包数据

            yield return StartCoroutine(fade.FadeIn()); // 渐入
        }

        private IEnumerator LoadMenu()  // 回到主菜单的协程
        {
            yield return SceneManager.LoadSceneAsync("Menu");
        }

        public void TransitionToFirstScene()    // 从主菜单去往第一个界面
        {
            StartCoroutine(MenuToScene("Game"));
        }

        public void TransitionToLoadScene() // 从主菜单点击consume
        {
            StartCoroutine(MenuToScene(SaveDataManager.Instance.SceneName));
        }

        public void TransitionToMenu()  // 回到主菜单
        {
            StartCoroutine(LoadMenu());
        }

        private IEnumerator EndNotifyDoing()    // 结束游戏时需要启用的协程
        {
            var fade = Instantiate(canvasGroupPrefab);  // 生成渐入渐出组件
            yield return StartCoroutine(fade.FadeOut());    // 渐出效果
            yield return StartCoroutine(LoadMenu());    // 回到Menu界面
            yield return StartCoroutine(fade.FadeIn()); // 渐入
        }
        public void EndNotify()
        {
            if (!_isFinished)
            {
                StartCoroutine(EndNotifyDoing());
                _isFinished = true;
            }
        }
        
    }// class SceneController
    
}// namespace Transition
