using CodeBase.Player;
using DG.Tweening;
using System;
using UnityEngine;

namespace CodeBase.Service
{
    public class GameManager : MonoBehaviour
    {
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage = default;

        [Header("Global Actions")]
        public static Action GameStartAction = default;
        public static Action PlayerLoadedAction = default;
        public static Action LevelPrepareAction = default;
        public static Action LevelReadyAction = default;
        public static Action LevelStartAction = default;

        [Header("Variables")] private bool playerIsLoaded = false;

        private void Start()
        {
            GameStartAction?.Invoke();

            Application.targetFrameRate = 60;
        }

        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            DOTween.SetTweensCapacity(1500, 50);
        }

        private void OnEnable()
        {
            playerStorage.LoadPlayer();

            LevelReadyAction += LevelReady;
            PlayerLoadedAction += PlayerLoaded;
        }

        private void OnDisable()
        {
            PlayerLoadedAction -= PlayerLoaded;
            LevelReadyAction -= LevelReady;
        }

        private void OnDestroy()
        {
            if (playerIsLoaded)
            {
                playerStorage.SavePlayer();
            }
        }

        private void OnApplicationQuit()
        {
            if (playerIsLoaded)
            {
                playerStorage.SavePlayer();
            }
        }

        private void OnApplicationPause(bool _pause)
        {
            if (_pause && playerIsLoaded)
            {
                playerStorage.SavePlayer();
            }
        }

        private void SavePlayer()
        {
            if (playerIsLoaded)
            {
                playerStorage.SavePlayer();
            }
        }

        private void LevelReady()
        {
            LevelStartAction?.Invoke();
        }

        private void PlayerLoaded()
        {
            playerIsLoaded = true;
            LevelPrepareAction?.Invoke();
        }
    }
}
