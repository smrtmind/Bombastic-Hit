using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float animationSpeed;
        [SerializeField] private float startPosition;

        [Header("Body Parts")]
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;

        [Space]
        [SerializeField] private Transform rightLeg;
        [SerializeField] private Transform leftLeg;

        private List<Coroutine> animationRoutines = new List<Coroutine>();

        private void OnEnable()
        {
            StartAnimation();
        }

        private void OnDisable()
        {
            EndAnimation();
        }

        private void StartAnimation()
        {
            animationRoutines.Add(StartCoroutine(AnimateObject(rightHand, -startPosition)));
            animationRoutines.Add(StartCoroutine(AnimateObject(leftHand, startPosition)));
            animationRoutines.Add(StartCoroutine(AnimateObject(rightLeg, startPosition)));
            animationRoutines.Add(StartCoroutine(AnimateObject(leftLeg, -startPosition)));
        }

        private void EndAnimation()
        {
            foreach (Coroutine animationRoutine in animationRoutines)
                StopCoroutine(animationRoutine);

            animationRoutines.Clear();
        }

        private IEnumerator AnimateObject(Transform bodyPart, float startPosition)
        {
            while (true)
            {
                float step = Mathf.PingPong(Time.time * animationSpeed, 1f);
                float rotation = Mathf.Lerp(startPosition, -startPosition, step);

                bodyPart.localRotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
