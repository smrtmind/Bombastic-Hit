using CodeBase.Service;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class LoadingScreen : SplashScreen
    {
        [SerializeField] private TextMeshProUGUI transitionText;

        public static Action<string, Color> OnLevelEnded;
        public static Action OnLoadingScreenActive;

        private void OnEnable()
        {
            OnLevelEnded += ShowScreen;
        }

        private void OnDisable()
        {
            OnLevelEnded -= ShowScreen;
        }

        protected void ShowScreen(string text, Color color)
        {
            StartCoroutine(StartTransitionVibration());

            base.ShowScreen();
            transitionText.text = text;
            transitionText.color = color;

            OnLoadingScreenActive?.Invoke();
        }

        private IEnumerator StartTransitionVibration()
        {
            yield return new WaitForSeconds(0.5f);
            VibrationController.Vibrate(500);
        }
    }
}
