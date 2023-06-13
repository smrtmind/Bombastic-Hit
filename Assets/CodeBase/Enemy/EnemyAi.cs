using CodeBase.Effects;
using CodeBase.ObjectBased;
using CodeBase.Player;
using CodeBase.Service;
using CodeBase.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using static Codebase.Utils.Enums;

namespace CodeBase.Enemy
{
    public class EnemyAi : MonoBehaviour
    {
        #region Variables
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;
        [SerializeField] private MaterialStorage materialStorage;

        [field: Header("Enemy Settings")]
        [field: SerializeField] public ColorType CurrentColor { get; private set; }
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float checkDistance;
        [SerializeField] private float sphereCastRadius = 1f;
        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private float timerToRespawn;
        [SerializeField] private MeshRenderer[] clothMeshes;

        [Header("Enemy Parts")]
        [SerializeField] private GameObject body;
        [SerializeField] private DroppableObject[] droppableObjects;

        public bool IsBusy { get; private set; }

        private Coroutine playerFollowRoutine;
        private Vector3 playerPosition;
        private ParticlePool particlePool;
        private ResourcePool resourcePool;
        private bool isDead;
        #endregion

        [Inject]
        private void Construct(PlayerController player, ParticlePool pPool, ResourcePool rPool)
        {
            playerPosition = player.transform.position;
            particlePool = pPool;
            resourcePool = rPool;
        }

        private void OnEnable()
        {
            InitRandomColor();

            UserInterface.OnEnemyTogglePressed += ReleaseImmidiate;
        }

        private void OnDisable()
        {
            UserInterface.OnEnemyTogglePressed -= ReleaseImmidiate;
        }

        private void InitRandomColor()
        {
            ColorData randomColorData = materialStorage.GetRandomColorData();

            CurrentColor = randomColorData.Type;

            foreach (var mesh in clothMeshes)
                mesh.material = randomColorData.Material;
        }

        private void Update()
        {
            if (!isDead)
            {
                if (Physics.CheckSphere(transform.position, sphereCastRadius, collisionLayer))
                {
                    particlePool.PlayParticleAction?.Invoke(transform.position, ParticleType.EnemyDead);
                    playerStorage.ModifyScore(playerStorage.ScoreOnKill);
                    UserInterface.OnScoreChanged?.Invoke();
                    Die();
                }
            }
        }

        private void Die()
        {
            isDead = true;
            agent.enabled = false;

            if (playerFollowRoutine != null)
            {
                StopCoroutine(playerFollowRoutine);
                playerFollowRoutine = null;
            }

            SpawnPopUp();
            body.SetActive(false);

            foreach (var droppable in droppableObjects)
                droppable.physicsObject.enabled = true;

            StartCoroutine(StartTimerBeforeRespawn());
        }

        private void ReleaseImmidiate()
        {
            if (!playerStorage.SpawnEnemies)
                Release();
        }

        private IEnumerator StartTimerBeforeRespawn()
        {
            yield return new WaitForSeconds(timerToRespawn);

            foreach (var droppable in droppableObjects)
            {
                droppable.physicsObject.enabled = false;
                droppable.physicsObject.transform.position = droppable.parentPoint.position;
                droppable.physicsObject.transform.rotation = droppable.parentPoint.rotation;
            }

            isDead = false;
            agent.enabled = true;
            Release();
        }

        public void Take()
        {
            IsBusy = true;

            if (!body.activeSelf)
                body.SetActive(true);

            gameObject.SetActive(true);
            playerFollowRoutine = StartCoroutine(MoveToCurrentDestination());
        }

        public void Release()
        {
            IsBusy = false;
            gameObject.SetActive(false);
        }

        private IEnumerator MoveToCurrentDestination()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                if (agent.destination != playerPosition)
                {
                    agent.destination = playerPosition;

                    if (Vector3.Distance(transform.position, playerPosition) <= checkDistance)
                    {
                        playerStorage.ModifyScore(-playerStorage.ScorePenalty);
                        UserInterface.OnScoreChanged?.Invoke();
                        DamageScreen.OnPlayerDamaged?.Invoke();
                        Release();
                        break;
                    }
                }
            }
        }

        private void SpawnPopUp()
        {
            var newPopUp = resourcePool.GetFreeResource(ResourceType.PopUp);
            if (newPopUp != null)
            {
                newPopUp.Take();
                newPopUp.transform.position = transform.position;
                newPopUp.transform.rotation = Quaternion.identity;
            }
        }

        [Serializable]
        public class DroppableObject
        {
            [field: SerializeField] public Transform parentPoint;
            [field: SerializeField] public PhysicsObject physicsObject;
        }
    }
}
