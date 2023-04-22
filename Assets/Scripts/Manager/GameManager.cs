using System.Collections.Generic;
using CharacterStatics.MonBehavior;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [HideInInspector]
        public CharacterStats playerStats;  // 玩家状态

        [HideInInspector] 
        public NavMeshAgent playerAgent;  // 玩家导航控制器

        private bool _isPlayerRegister; // 玩家是否已经注册了
        private CinemachineFreeLook _freeCamera;    // freeLook摄像机

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private List<IEndGameObserver> _endGameObservers = new List<IEndGameObserver>();    // 观察者列表
        public bool IsPlayerDead => playerStats == null || playerStats.CurrentHealth <= 0;    // 玩家是否死亡

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);    // 加载场景的时候不被删除
        }

        private void Update()
        {
            if (_isPlayerRegister && IsPlayerDead)
            {
                NotifyObservers();
            }
        }

        public void RegisterPlayer(CharacterStats characterStats)   // 玩家信息注册
        {
            playerStats = characterStats;
            playerAgent = playerStats.gameObject.GetComponent<NavMeshAgent>();
            _isPlayerRegister = true;
            _freeCamera = FindObjectOfType<CinemachineFreeLook>();
            if (_freeCamera == null) return;
            var lookAtPoint = playerStats.transform.GetChild(2).transform;
            _freeCamera.Follow = lookAtPoint;
            _freeCamera.LookAt = lookAtPoint;
        }

        public void AddObserver(IEndGameObserver observer)    // 观察者信息注册
        {
            if (observer == null) return;
            if (!IsInitialized) return;
            _endGameObservers.Add(observer);
        }

        public void RemoveObserver(IEndGameObserver observer)   // 观察者信息移出
        {
            if (observer == null) return;
            _endGameObservers.Remove(observer);
        }

        private void NotifyObservers()   // 广播信息
        {
            foreach (var observer in _endGameObservers)
            {
                observer.EndNotify();
            }
        }

    }// class GameManager
    
}// namespace Manager
