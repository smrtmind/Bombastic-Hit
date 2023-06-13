using CodeBase.Service;
using System;
using UnityEngine;
using Zenject;
using static Codebase.Utils.Enums;
using Random = UnityEngine.Random;

namespace CodeBase.ObjectBased
{
    public class PhysicsObject : MonoBehaviour
    {
        #region Variables
        [Header("Storages")]
        [SerializeField] private ResourceStorage resourceStorage;

        [Header("Physic Settings")]
        [SerializeField] private float mass = 1f;
        [SerializeField] private float drag = 0f;
        [SerializeField] private float angularDrag = 0.05f;
        [SerializeField] private bool useGravity = true;
        [SerializeField] private bool isKinematic = false;
        [SerializeField] private float speedReductionFactor = 0.8f;
        [SerializeField] private int maxRicochets = 3;
        [SerializeField] private LayerMask layersToRicochet;
        [SerializeField] private bool canLeaveMarks;

        public event Action OnReachedMaxRicochets;

        private Vector3 velocity;
        private Vector3 angularVelocity;
        private int ricochetCount = 0;
        private float fixedTimeStep = 0.01f;
        private float accumulatedTime = 0f;
        private ResourcePool resourcePool;
        #endregion

        [Inject]
        private void Construct(ResourcePool rPool)
        {
            resourcePool = rPool;
        }

        private void OnEnable()
        {
            velocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            ricochetCount = 0;
        }

        private void Update()
        {
            accumulatedTime += Time.deltaTime;

            while (accumulatedTime >= fixedTimeStep)
            {
                PerformPhysicsStep(fixedTimeStep);
                accumulatedTime -= fixedTimeStep;
            }
        }

        private void PerformPhysicsStep(float deltaTime)
        {
            if (!isKinematic)
            {
                UpdatePosition(deltaTime);
                ResolveCollisions();
            }
        }

        private void UpdatePosition(float deltaTime)
        {
            // Update position based on velocity
            transform.position += velocity * deltaTime;
            // Apply drag to slow down the velocity
            velocity *= Mathf.Clamp01(1f - drag * deltaTime);
            // Update rotation based on angular velocity
            transform.rotation *= Quaternion.Euler(angularVelocity * deltaTime);
            // Apply angular drag to slow down the angular velocity
            angularVelocity *= Mathf.Clamp01(1f - angularDrag * deltaTime);

            if (useGravity)
            {
                velocity += Physics.gravity * deltaTime;
            }
        }

        private void ResolveCollisions()
        {
            if (Physics.SphereCast(transform.position, 0.25f, velocity.normalized, out RaycastHit hit, velocity.magnitude * fixedTimeStep, layersToRicochet))
            {
                Vector3 reflectionDirection = Vector3.Reflect(velocity.normalized, hit.normal);
                velocity = reflectionDirection * velocity.magnitude;
                velocity *= speedReductionFactor;

                if (canLeaveMarks)
                {
                    ricochetCount++;
                    if (ricochetCount == maxRicochets)
                    {
                        SpawnMark(hit.point, hit.normal, ResourceType.WallDamage);
                        OnReachedMaxRicochets?.Invoke();
                    }
                    else
                    {
                        SpawnMark(hit.point, hit.normal, ResourceType.WallCrack);
                    }
                }
            }
        }

        private void SpawnMark(Vector3 position, Vector3 normal, ResourceType markType)
        {
            var newMark = resourcePool.GetFreeResource(markType);
            if (newMark != null)
            {
                newMark.Take();
                newMark.transform.position = position;
                newMark.transform.rotation = Quaternion.LookRotation(normal);

                float randomScale = Random.Range(0.5f, 1f);
                newMark.transform.localScale = new Vector3(randomScale, randomScale, 1f);
            }
        }

        public void AddForce(Vector3 force) => velocity += force / mass;

        public void AddTorque(Vector3 torque) => angularVelocity += torque / mass;
    }
}
