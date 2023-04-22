using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        private Text _levelText;
        private Image _healthSlider;
        private Image _expSlider;

        private void Awake()
        {
            _levelText = transform.GetChild(2).GetComponent<Text>();
            _healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            _expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        }

        private void Update()
        {
            UpdateHealthBar();
            UpdateExpBar();
            UpdateLevelText();
        }

        private void UpdateHealthBar()  // 更新血量显示
        {
            var playerCharacterStats = GameManager.Instance.playerStats;
            var percent = 1.0f * playerCharacterStats.CurrentHealth / playerCharacterStats.MaxHealth;
            _healthSlider.fillAmount = percent;
        }

        private void UpdateExpBar() // 更新经验显示
        {
            var playerCharacterStats = GameManager.Instance.playerStats;
            var percent = 1.0f * playerCharacterStats.CurrentExperience / playerCharacterStats.BaseExperience;
            _expSlider.fillAmount = percent;
        }

        private void UpdateLevelText()  // 更新等级显示
        {
            var playerCharacterStats = GameManager.Instance.playerStats;
            _levelText.text = "Level:  " + playerCharacterStats.CurrentLevel.ToString("00");
        }
        
    }// class PlayerHealthUI
}// namespace UI
