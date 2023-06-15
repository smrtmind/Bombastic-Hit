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

        [Header("Cannon Settings")]
        [SerializeField] private Transform cannon;
        [field: SerializeField] public Transform ShotPoint { get; private set; }
        [SerializeField] private ParticleSystem cannonBurst;

        private bool readyToShoot = true;
        private ResourcePool resourcePool;
        private Tween cannonAnimationTween;
        #endregion

        [Inject]
        private void Construct(ResourcePool rPool)
        {
            resourcePool = rPool;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MoveCannon();
                ShootCannonball();
            }
        }

        private void MoveCannon()
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            float mouseRotationSpeed = 1f;

            transform.rotation *= Quaternion.Euler(new Vector3(0f * mouseRotationSpeed, deltaX * mouseRotationSpeed, 0f));
            cannon.rotation *= Quaternion.Euler(new Vector3(0f * mouseRotationSpeed, 0f, deltaY * mouseRotationSpeed));
        }

        private void ShootCannonball()
        {
            if (readyToShoot)
            {
                readyToShoot = false;

                cannonAnimationTween?.Kill();
                cannonAnimationTween = cannon.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), playerStorage.PlayerData.ShootingDelay)
                    .OnComplete(() => readyToShoot = true);

                cannonBurst.Play();
                //CameraShaker.OnShakeCamera?.Invoke();

                var newCannonBall = resourcePool.GetFreeResource(ResourceType.Ammo);
                if (newCannonBall != null && newCannonBall is CannonBall ball)
                {
                    ball.transform.position = ShotPoint.position;
                    ball.Take();
                    ball.Throw(ShotPoint.transform.up * playerStorage.PlayerData.ShootingPower);
                }
            }
        }
    }
}
