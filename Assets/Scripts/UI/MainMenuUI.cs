using Transition;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private Button _newGameButton;  // 新游戏
        private Button _consumeButton;  // 继续游戏
        private Button _quitButton; // 退出游戏
        private PlayableDirector _director; // timeline控制器

        private void Awake()
        {
            // 获取Button组件
            _newGameButton = transform.GetChild(1).GetComponent<Button>();
            _consumeButton = transform.GetChild(2).GetComponent<Button>();
            _quitButton = transform.GetChild(3).GetComponent<Button>();
            _director = FindObjectOfType<PlayableDirector>();
            
            // 添加button对应关联的事件
            _newGameButton.onClick.AddListener(PlayTimeLine);
            _director.stopped += NewGame;   // 先播放timeline, 再开始新游戏
            
            _consumeButton.onClick.AddListener(Consume);
            _quitButton.onClick.AddListener(Quit);
        }

        private void NewGame(PlayableDirector director)
        {
            PlayerPrefs.DeleteAll();    // 删除记录
            SceneController.Instance.TransitionToFirstScene();  // 转换场景
        }

        private void Consume()
        {
            SceneController.Instance.TransitionToLoadScene();   // 转换场景
        }

        private void Quit()
        {
            Application.Quit();
        }

        private void PlayTimeLine()
        {
            _director.Play();
        }
        
    }// class MainMenuUI
    
}// namespace UI
