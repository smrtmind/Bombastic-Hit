using CodeBase.Player;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Codebase.Utils.Enums;

namespace CodeBase.UI
{
    public class GamePanelController : MonoBehaviour, IInterfacePanel
    {
        #region Variables
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;

        [Header("Base")]
        [SerializeField] private UIPanelType uiPanelType = UIPanelType.Game;
        [SerializeField] private RectTransform panelContainer = default;

        [Header("UI Settings")]
        [SerializeField] private TextMeshProUGUI scoreValue;
        [SerializeField] private TextMeshProUGUI lvlValue;
        [SerializeField] private TextMeshProUGUI lvlProgressValue;
        [SerializeField] private Slider lvlProgressSlider;
        [SerializeField] private Sprite fullHeart;
        [SerializeField] private Sprite brokenHeart;
        [SerializeField] private List<Image> healthHearts;

        public static Action OnScoreChanged;
        public static Action OnHealthChanged;
        public static Action OnLevelChanged;
        public static Action OnLevelProgressChanged;

        private Tween progressSliderTween;
        #endregion

        private void OnEnable()
        {
            OnScoreChanged += UpdateScore;
            OnHealthChanged += UpdateHealth;
            OnLevelChanged += UpdateLevel;
            OnLevelProgressChanged += UpdateLevelProgress;
        }

        private void OnDisable()
        {
            OnScoreChanged -= UpdateScore;
            OnHealthChanged -= UpdateHealth;
            OnLevelChanged -= UpdateLevel;
            OnLevelProgressChanged -= UpdateLevelProgress;
        }

        private void Start()
        {
            lvlProgressSlider.minValue = 0;
            lvlProgressSlider.maxValue = playerStorage.PlayerData.EnemyAmount;
            lvlProgressSlider.value = 0;

            UpdateHealth();
            UpdateScore();
            UpdateLevel();
            UpdateLevelProgress();
        }

        private void UpdateScore() => scoreValue.text = $"{playerStorage.PlayerData.Score}";

        private void UpdateHealth()
        {
            int filledHearts = playerStorage.PlayerData.CurrentHealth;
            int brokenHearts = healthHearts.Count - filledHearts;

            for (int i = 0; i < healthHearts.Count; i++)
            {
                if (i < filledHearts)
                    healthHearts[i].sprite = fullHeart;
                else if (i < filledHearts + brokenHearts)
                    healthHearts[i].sprite = brokenHeart;
            }
        }

        private void UpdateLevel()
        {
            lvlValue.text = $"lvl: {playerStorage.PlayerData.Level}";
            lvlProgressSlider.maxValue = playerStorage.PlayerData.EnemyAmount;

            playerStorage.SavePlayer();
        }

        private void UpdateLevelProgress()
        {
            lvlProgressValue.text = $"{playerStorage.PlayerData.CurrentEnemyAmount} / {playerStorage.PlayerData.EnemyAmount}";

            progressSliderTween?.Kill();
            progressSliderTween = lvlProgressSlider.DOValue(playerStorage.PlayerData.CurrentEnemyAmount, 0.1f);
        }

        private void Awake()
        {
            Init();
        }

        #region IInterfacePanel
        public UIPanelType UIPanelType { get => uiPanelType; }

        public void Hide()
        {
            panelContainer.gameObject.SetActive(false);
        }

        public void Show()
        {
            panelContainer.gameObject.SetActive(true);
        }

        public void Init()
        {
            UIController.InterfacePanels.Add(this);
        }
        #endregion
    }
}
