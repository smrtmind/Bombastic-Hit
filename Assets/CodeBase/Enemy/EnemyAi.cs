using CodeBase.Effects;
using CodeBase.ObjectBased;
using CodeBase.Player;
using CodeBase.Service;
using CodeBase.UI;
using CodeBase.Utils;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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
        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private float timerToRespawn;
        [SerializeField] private MeshRenderer[] clothMeshes;
        [SerializeField] private Collider mainCollider;
        [SerializeField] private Image markerCircle;
        [SerializeField] private TutorialInfo tutorialInfo;

        [Header("Enemy Parts")]
        [SerializeField] private GameObject body;
        [SerializeField] private DroppableObject[] droppableObjects;

        public bool IsBusy { get; private set; }

        private Coroutine playerFollowRoutine;
        private Vector3 playerPosition;
        private ParticlePool particlePool;
        private ResourcePool resourcePool;
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
            RepaintEnemy(ColorType.Random);

            if (!playerStorage.PlayerData.TutorialCompleted)
                tutorialInfo.gameObject.SetActive(true);

            LoadingScreen.OnLoadingScreenActive += ReleaseImmidiate;
        }

        private void OnDisable()
        {
            if (tutorialInfo.gameObject.activeSelf)
                tutorialInfo.gameObject.SetActive(false);

            LoadingScreen.OnLoadingScreenActive -= ReleaseImmidiate;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag.Equals(Tags.Weapon))
            {
                var ball = Dictionaries.CannonBalls.FirstOrDefault(ball => ball.Key == collision.gameObject.transform);
                if (CurrentColor == ball.Value.CurrentColor)
                {
                    particlePool.PlayParticleAction?.Invoke(transform.position, ParticleType.EnemyDead);
                    playerStorage.PlayerData.ModifyScore();
                    playerStorage.PlayerData.ModifyEnemyAmount();
                    Die();
                }
                else
                {
                    CurrentColor = ball.Value.CurrentColor;
                    RepaintEnemy(CurrentColor);
                }
            }
        }

        private void RepaintEnemy(ColorType colorType)
        {
            ColorData randomColorData = materialStorage.GetColorData(colorType);

            CurrentColor = randomColorData.Type;

            foreach (var mesh in clothMeshes)
                mesh.material = randomColorData.Material;

            if (ColorUtility.TryParseHtmlString(CurrentColor.ToString(), out Color color))
            {
                markerCircle.color = color;
                tutorialInfo.SetText($"need <color={CurrentColor}>{CurrentColor}</color> ball");
            }
        }

        private void Die()
        {
            mainCollider.enabled = false;
            agent.enabled = false;

            if (playerFollowRoutine != null)
            {
                StopCoroutine(playerFollowRoutine);
                playerFollowRoutine = null;
            }

            SpawnPopUp();
            body.SetActive(false);

            foreach (var droppable in droppableObjects)
                droppable.Rb.isKinematic = false;

            if (tutorialInfo.gameObject.activeSelf)
                tutorialInfo.gameObject.SetActive(false);

            StartCoroutine(StartTimerBeforeRespawn(timerToRespawn));
        }

        private void ReleaseImmidiate() => StartCoroutine(StartTimerBeforeRespawn(0f));

        private IEnumerator StartTimerBeforeRespawn(float timer)
        {
            yield return new WaitForSeconds(timer);

            foreach (var droppable in droppableObjects)
            {
                droppable.Rb.isKinematic = true;
                droppable.Rb.transform.position = droppable.ParentPoint.position;
                droppable.Rb.transform.rotation = droppable.ParentPoint.rotation;
            }

            mainCollider.enabled = true;
            Release();
        }

        public void Take()
        {
            IsBusy = true;

            if (!body.activeSelf)
                body.SetActive(true);

            gameObject.SetActive(true);
            agent.enabled = true;
            playerFollowRoutine = StartCoroutine(MoveToCurrentDestination());
        }

        public void Release()
        {
            IsBusy = false;
            agent.enabled = false;
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
                        playerStorage.PlayerData.ModifyHealth();
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
            if (newPopUp != null && newPopUp is PopUp popUp)
            {
                popUp.Take();
                popUp.transform.position = transform.position;
                popUp.transform.rotation = Quaternion.identity;
                popUp.Spawn($"+{playerStorage.PlayerData.ScoreOnKill}", "yellow");
            }
        }

        [Serializable]
        public class DroppableObject
        {
            [field: SerializeField] public Transform ParentPoint { get; private set; }
            [field: SerializeField] public Rigidbody Rb { get; private set; }
        }
    }
}
