using CodeBase.Player;
using CodeBase.Service;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Codebase.Utils.Enums;

namespace CodeBase.UI
{
    public class PromoPanelController : MonoBehaviour, IInterfacePanel
    {
        [Header("Base")]
        [SerializeField] private UIPanelType uiPanelType = UIPanelType.Promo;
        [SerializeField] private Transform panelContainer = default;
        [SerializeField] private PlayerStorage playerStorage = default;

        [Header("Components")]
        [SerializeField] private CanvasGroup canvasGroupPromo = default;

        [Header("Loading Setiings")]
        [SerializeField] private Image loadingFiller;
        [SerializeField, Min(1f)] private float loadingDelay;

        private Camera mainCamera;

        [Inject]
        private void Construct(CameraController cameraController)
        {
            mainCamera = cameraController.MainCamera;
        }

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            loadingFiller.fillAmount = 0f;

            GameManager.GameStartAction += ShowPromo;
        }

        private void OnDisable()
        {
            GameManager.GameStartAction -= ShowPromo;
        }

        private void ShowPromo()
        {
            canvasGroupPromo.alpha = 1f;
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Promo);
        }

        #region IInterfacePanel
        public UIPanelType UIPanelType { get => uiPanelType; }

        public void Hide()
        {
            panelContainer.gameObject.SetActive(false);
        }

        public void Show()
        {
            if (panelContainer != null)
                panelContainer.gameObject.SetActive(true);

            loadingFiller.DOFillAmount(1f, loadingDelay)
                .OnComplete(() =>
                {
                    StartCoroutine(FinallyLoading(() => {
                        DOTween.To(() => canvasGroupPromo.alpha, (_alpha) => canvasGroupPromo.alpha = _alpha, 0f, 0.5f)
                            .SetEase(Ease.Flash)
                            .OnComplete(() =>
                            {
                                UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Game);
                            });
                    }));
                });

            StartCoroutine(GameLoading());
            StartCoroutine(LoadPlayer());
        }

        private IEnumerator LoadPlayer()
        {
            yield return new WaitForSeconds(0.1f);
            playerStorage.LoadPlayer();
            yield return null;
            GameManager.PlayerLoadedAction?.Invoke();
            //GameManager.PrepareLevelAction?.Invoke();
        }

        private IEnumerator GameLoading()
        {
            mainCamera.enabled = false;

            yield return new WaitForSeconds(0.1f);

            //yield return new WaitUntil(() => playerStorageSO.ConcretePlayerData != null);
        }

        private IEnumerator FinallyLoading(Action finishLoading)
        {
            //yield return new WaitUntil(() => playerStorageSO.ConcretePlayerData != null);
            GameManager.LevelStartAction?.Invoke();
            mainCamera.enabled = true;
            yield return new WaitForSeconds(0.3f);
            finishLoading?.Invoke();
        }

        public void Init()
        {
            UIController.InterfacePanels.Add(this);
        }
        #endregion
    }
}
