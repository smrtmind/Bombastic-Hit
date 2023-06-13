using System;
using System.Collections;
using UnityEngine;

namespace CodeBase.UI
{
    public class DamageScreen : MonoBehaviour
    {
        [SerializeField] private float activeDuration = 0.25f;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private CanvasGroup screenCanvasGroup;

        public static Action OnPlayerDamaged;

        private Coroutine damageScreenRoutine;

        private void OnEnable()
        {
            OnPlayerDamaged += ShowDamageScreen;
        }

        private void OnDisable()
        {
            OnPlayerDamaged -= ShowDamageScreen;
        }

        private void ShowDamageScreen()
        {
            if (damageScreenRoutine != null)
            {
                StopCoroutine(damageScreenRoutine);
                damageScreenRoutine = null;
            }

            damageScreenRoutine = StartCoroutine(ShowDamageCoroutine());
        }

        private IEnumerator ShowDamageCoroutine()
        {
            screenCanvasGroup.alpha = 1f;

            yield return new WaitForSeconds(activeDuration);

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                screenCanvasGroup.alpha = alpha;
                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            screenCanvasGroup.alpha = 0f;
            damageScreenRoutine = null;
        }
    }
}
