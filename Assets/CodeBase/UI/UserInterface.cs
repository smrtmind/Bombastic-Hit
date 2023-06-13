using CodeBase.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class UserInterface : MonoBehaviour
    {
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;

        [Header("UI Settings")]
        [SerializeField] private Slider powerSlider;
        [SerializeField] private TextMeshProUGUI powerValue;
        [SerializeField] private Toggle enemySpawnToggle;
        [SerializeField] private TextMeshProUGUI scoreValue;

        public static Action OnEnemyTogglePressed;
        public static Action OnScoreChanged;

        private void OnEnable()
        {
            powerSlider.onValueChanged.AddListener(UpdatePowerValue);
            enemySpawnToggle.onValueChanged.AddListener(UpdateEnemySpawnToggle);

            OnScoreChanged += UpdateScore;
        }

        private void OnDisable()
        {
            powerSlider.onValueChanged.RemoveListener(UpdatePowerValue);
            enemySpawnToggle.onValueChanged.RemoveListener(UpdateEnemySpawnToggle);

            OnScoreChanged -= UpdateScore;
        }

        private void Start()
        {
            InitSlider();
            UpdateScore();
            enemySpawnToggle.isOn = playerStorage.SpawnEnemies;
        }

        private void InitSlider()
        {
            if (powerSlider != null)
            {
                powerSlider.minValue = 0f;
                powerSlider.maxValue = playerStorage.MaxShootingPower;
                powerSlider.value = playerStorage.CurrentShootingPower;
            }
        }

        private void UpdatePowerValue(float value)
        {
            powerValue.text = $"{Mathf.Round(value)}";
            playerStorage.ModifyShootingPowerValue(value);
        }

        private void UpdateEnemySpawnToggle(bool enable)
        {
            scoreValue.gameObject.SetActive(enable);

            playerStorage.ModifyEnemySpawner(enable);
            OnEnemyTogglePressed?.Invoke();
        }

        private void UpdateScore() => scoreValue.text = $"score: {playerStorage.Score}";
    }
}
