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
        public static Action StartCollectPeriodAction = default;
        public static Action RestartCollectPeriodAction = default;
        public static Action StartWarPeriodAction = default;
        //public static Action<LevelResult> LevelFinishAction = default;

        [Header("Variables")] private bool playerIsLoaded = false;
        //private float startLevelTime = default;

        private void Start()
        {
            GameStartAction?.Invoke();
        }

        private void Awake()
        {
            //playerStorageSO.LoadPlayer();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            DOTween.SetTweensCapacity(1500, 50);
        }

        private void OnEnable()
        {
            playerStorage.LoadPlayer();
            //LevelStartAction += StartLevel;
            LevelReadyAction += LevelReady;
            //LevelFinishAction += LevelFinish;
            PlayerLoadedAction += PlayerLoaded;
            //GameStartAction += StartGame;
        }

        //private void Awake()
        //{
        //    playerStorageSO.LoadPlayer();
        //    GameStartAction?.Invoke();
        //}

        private void OnDisable()
        {
            //LevelStartAction -= StartLevel;
            PlayerLoadedAction -= PlayerLoaded;
            LevelReadyAction -= LevelReady;
            //LevelFinishAction -= LevelFinish;
            //GameStartAction -= StartGame;
        }

        //private void StartLevel()
        //{
        //    GamePanelController.OnPointsChanged?.Invoke();
        //}

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
