using CodeBase.ObjectBased;
using CodeBase.Player;
using CodeBase.Service;
using CodeBase.Utils;
using DG.Tweening;
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
        [SerializeField] private float endHeight;
        [SerializeField] private Vector3 startScale = Vector3.zero;
        [SerializeField] private float dissapearDuration = 1f;

        private Camera mainCamera;
        private Color defaultPopUpColor;

        [Inject]
        private void Construct(CameraController cameraController)
        {
            mainCamera = cameraController.MainCamera;
        }

        private void Awake()
        {
            defaultPopUpColor = ValueInfo.color;
        }

        public override void Take()
        {
            base.Take();
            gameObject.SetActive(true);
        }

        public override void Release()
        {
            base.Release();
            gameObject.SetActive(false);
        }

        public void Spawn(string text, string color)
        {
            ValueInfo.color = defaultPopUpColor;
            transform.localScale = Vector3.zero;
            ValueInfo.text = $"<color={color}>{text}</color>";

            gameObject.SetActive(true);
            StartCoroutine(SpecialCoroutine.LookAt(transform, mainCamera.transform));

            Sequence popUpAppear = DOTween.Sequence().SetAutoKill();
            popUpAppear.Append(transform.DOScale(startScale, 0.25f))
                       .Append(transform.DOScale(Vector3.one, 0.25f))
                       .OnComplete(() => FadePopUp());
        }

        private void FadePopUp()
        {
            transform.DOMoveY(transform.position.y + endHeight, dissapearDuration).SetEase(Ease.Linear).SetAutoKill();
            ValueInfo.DOColor(new Color(ValueInfo.color.r, ValueInfo.color.g, ValueInfo.color.b, 0f), dissapearDuration)
                     .OnComplete(() => Release()).SetAutoKill();
        }
    }
}
