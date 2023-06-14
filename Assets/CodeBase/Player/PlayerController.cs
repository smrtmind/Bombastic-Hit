﻿using CodeBase.ObjectBased;
using CodeBase.Service;
using System.Collections;
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
        private Vector3 defaultCannonPosition;
        private ResourcePool resourcePool;
        #endregion

        [Inject]
        private void Construct(ResourcePool rPool)
        {
            resourcePool = rPool;
        }

        private void Start()
        {
            defaultCannonPosition = cannon.localPosition;
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
                StartCoroutine(ShotAnimation());

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

        private IEnumerator ShotAnimation()
        {
            yield return StartCoroutine(CannonAnimation());
            readyToShoot = true;
        }

        private IEnumerator CannonAnimation()
        {
            while (cannon.localPosition.x > -2f)
            {
                yield return cannon.localPosition -= new Vector3(Time.deltaTime * 8f, 0f, 0f);
            }

            while (cannon.localPosition.x < defaultCannonPosition.x)
            {
                yield return cannon.localPosition += new Vector3(Time.deltaTime * 8f, 0f, 0f);
            }
        }
    }
}
