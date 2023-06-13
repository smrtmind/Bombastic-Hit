using CodeBase.ObjectBased;
using CodeBase.Player;
using CodeBase.Service;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI
{
    public class PopUp : ResourceUnit
    {
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;

        [field: Space]
        [field: SerializeField] public TextMeshProUGUI ValueInfo { get; private set; }

        [Space]
        [SerializeField] private float maxScale;
        [SerializeField] private float scaleSpeed;

        [Space]
        [SerializeField] private float maxHeight;
        [SerializeField] private float moveSpeed;

        private Camera mainCamera;

        [Inject]
        private void Construct(CameraController cameraController)
        {
            mainCamera = cameraController.MainCamera;
        }

        public override void Take()
        {
            base.Take();

            gameObject.SetActive(true);
            Spawn();
        }

        public override void Release()
        {
            base.Release();

            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        public void Spawn()
        {
            ValueInfo.text = $"+{playerStorage.ScoreOnKill}";

            transform.localScale = new Vector3(0f, 0f, 1f);

            StartCoroutine(LookAt(transform, mainCamera.transform));
            StartCoroutine(StartActiveBehaviourRoutine());
        }

        private IEnumerator StartActiveBehaviourRoutine()
        {
            yield return StartCoroutine(ScalePopUp());
            yield return StartCoroutine(MovePopUp());

            Release();
        }

        private IEnumerator ScalePopUp()
        {
            while (transform.localScale.x < maxScale)
            {
                yield return transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * scaleSpeed, transform.localScale.y + Time.deltaTime * scaleSpeed, transform.localScale.z);
            }
        }

        private IEnumerator MovePopUp()
        {
            while (transform.position.y < maxHeight)
            {
                yield return transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * moveSpeed, transform.position.z);
            }
        }

        private IEnumerator LookAt(Transform who, Transform where)
        {
            while (true)
            {
                who.LookAt(where.position);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
