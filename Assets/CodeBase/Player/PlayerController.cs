using CodeBase.Effects;
using CodeBase.ObjectBased;
using CodeBase.Service;
using DG.Tweening;
using UnityEngine;
using Zenject;
using static Codebase.Utils.Enums;

namespace CodeBase.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;
        [SerializeField] private MaterialStorage materialStorage;

        [Header("Cannon Settings")]
        [SerializeField] private Transform cannon;
        [field: SerializeField] public Transform ShotPoint { get; private set; }
        [SerializeField] private SpriteRenderer groundMarkerRenderer;
        [SerializeField] private SpriteRenderer aimRenderer;
        [SerializeField] private Transform aimTransform;

        private bool readyToShoot = true;
        private ResourcePool resourcePool;
        private Tween cannonAnimationTween;
        private Camera mainCamera;
        private bool isAiming;
        private bool colorIsGenerated;
        private ColorData randomColorData;
        private ParticlePool particlePool;
        private Ray mouseRay;
        private float rayDistance;
        private Plane groundPlane;
        private Vector3 targetPoint;
        #endregion

        [Inject]
        private void Construct(ResourcePool rPool, CameraController cameraController, ParticlePool pPool)
        {
            resourcePool = rPool;
            mainCamera = cameraController.MainCamera;
            particlePool = pPool;
        }

        private void Start()
        {
            aimTransform.SetParent(resourcePool.transform);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isAiming = true;
                aimRenderer.gameObject.SetActive(true);

                if (!colorIsGenerated)
                {
                    colorIsGenerated = true;
                    GenerateBallColor();
                }
            }
            else if (Input.GetMouseButtonUp(0) && readyToShoot)
            {
                readyToShoot = false;
                isAiming = false;
                aimRenderer.gameObject.SetActive(false);
                ShootCannonball();
            }

            if (isAiming)
                AimCannon();
        }

        private void GenerateBallColor()
        {
            randomColorData = materialStorage.GetColorData(ColorType.Random);

            if (ColorUtility.TryParseHtmlString(randomColorData.Type.ToString(), out Color color))
            {
                groundMarkerRenderer.color = color;
                groundMarkerRenderer.gameObject.SetActive(true);

                aimRenderer.color = color;
                aimRenderer.gameObject.SetActive(true);
            }
        }

        private void AimCannon()
        {
            mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(mouseRay, out rayDistance))
            {
                targetPoint = mouseRay.GetPoint(rayDistance);
                targetPoint.y = transform.position.y;

                aimRenderer.gameObject.transform.position = targetPoint;

                Vector3 direction = targetPoint - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = lookRotation * Quaternion.Euler(0f, -90f, 0f);
            }
        }

        private void ShootCannonball()
        {
            ShowVfx();

            targetPoint.y = ShotPoint.position.y;
            Vector3 initialVelocity = CalculateProjectileVelocity(ShotPoint.position, targetPoint, playerStorage.PlayerData.ShootingPower);

            var newCannonBall = resourcePool.GetFreeResource(ResourceType.Ammo);
            if (newCannonBall != null && newCannonBall is CannonBall ball)
            {
                ball.transform.position = ShotPoint.position;
                ball.Take();
                ball.RepaintCannonBall(randomColorData);
                ball.Throw(initialVelocity);
            }

            groundMarkerRenderer.gameObject.SetActive(false);
            colorIsGenerated = false;
        }

        private Vector3 CalculateProjectileVelocity(Vector3 origin, Vector3 target, float launchSpeed)
        {
            Vector3 displacement = target - origin;
            float time = displacement.magnitude / launchSpeed;
            Vector3 velocity = displacement / time;
            velocity.y += Mathf.Abs(Physics.gravity.y) * time * 0.5f;

            return velocity;
        }

        private void ShowVfx()
        {
            cannonAnimationTween?.Kill();
            cannonAnimationTween = cannon.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.1f)
                .OnComplete(() => readyToShoot = true);

            particlePool.PlayParticleAction?.Invoke(ShotPoint.position, ParticleType.SmokePuff);
        }
    }
}
