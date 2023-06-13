using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Service
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float force = 0.25f;

        public static Action OnShakeCamera;

        private Vector3 defaultPosition;
        private Coroutine shakeCoroutine;

        private void OnEnable()
        {
            OnShakeCamera += Shake;
        }

        private void OnDisable()
        {
            OnShakeCamera -= Shake;
        }

        private void Start()
        {
            defaultPosition = transform.localPosition;
        }

        private void Shake()
        {
            if (shakeCoroutine == null)
                shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, force));
        }

        private IEnumerator ShakeCoroutine(float duration, float force)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                Vector3 randomOffset = Random.insideUnitSphere * force;
                transform.localPosition = defaultPosition + randomOffset;
                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            transform.localPosition = defaultPosition;
            shakeCoroutine = null;
        }
    }
}
