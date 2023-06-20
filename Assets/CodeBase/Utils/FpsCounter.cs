using System.Collections;
using TMPro;
using UnityEngine;

namespace CodeBase.Utils
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsValue;
        [SerializeField] private Color fpsColor;
        [SerializeField] private float fpsRefreshRate = 0.5f;

        private float deltaTime = 0.0f;

        private void Start()
        {
            fpsValue.color = fpsColor;
            StartCoroutine(RefreshFPSCounter());
        }

        private IEnumerator RefreshFPSCounter()
        {
            while (true)
            {
                yield return new WaitForSeconds(fpsRefreshRate);

                float fps = 1.0f / deltaTime;
                fpsValue.text = $"fps: {Mathf.Round(fps)}";
            }
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
    }
}
