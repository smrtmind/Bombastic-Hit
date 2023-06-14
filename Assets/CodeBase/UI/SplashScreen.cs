using System.Collections;
using UnityEngine;

namespace CodeBase.UI
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private float activeDuration = 0.25f;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private CanvasGroup screenCanvasGroup;

        protected Coroutine damageScreenRoutine;

        protected virtual void ShowScreen()
        {
            if (damageScreenRoutine != null)
            {
                StopCoroutine(damageScreenRoutine);
                damageScreenRoutine = null;
            }

            damageScreenRoutine = StartCoroutine(StartScreenCoroutine());
        }

        private IEnumerator StartScreenCoroutine()
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
