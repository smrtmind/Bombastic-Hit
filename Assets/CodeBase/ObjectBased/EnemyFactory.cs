using CodeBase.Enemy;
using CodeBase.Player;
using CodeBase.Service;
using CodeBase.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CodeBase.ObjectBased
{
    public class EnemyFactory : MonoBehaviour
    {
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;

        [Header("Factory Settings")]
        [SerializeField] private float spawnDelay;
        [SerializeField] private EnemyAi enemyPrefab;
        [SerializeField] private Transform enemyContainer;

        [Inject] private DiContainer diContainer;

        private List<EnemyAi> enemyPool = new List<EnemyAi>();
        private Coroutine enemySpawnRoutine;
        private List<Transform> spawnPoints = new List<Transform>();
        private LocationController locationController;

        [Inject]
        private void Construct(LocationController lController)
        {
            locationController = lController;
        }

        private void OnEnable()
        {
            GameManager.LevelStartAction += StartSpawner;
            GamePanelController.OnLevelChanged += StartSpawner;
        }

        private void OnDisable()
        {
            GameManager.LevelStartAction -= StartSpawner;
            GamePanelController.OnLevelChanged -= StartSpawner;
        }

        private void StartSpawner()
        {
            if (enemySpawnRoutine != null)
                StopCoroutine(enemySpawnRoutine);

            spawnPoints.Clear();
            spawnPoints = locationController.GetCurrentSpawnPoints();

            enemySpawnRoutine = StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                SpawnNewEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        private void SpawnNewEnemy()
        {
            EnemyAi newEnemy = GetFreeEnemy();
            if (newEnemy != null)
            {
                newEnemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
                newEnemy.Take();
            }
        }

        private EnemyAi GetFreeEnemy()
        {
            EnemyAi freeEnemy = enemyPool.Find(ammo => !ammo.IsBusy);
            if (freeEnemy == null)
                freeEnemy = CreateNewEnemy();

            return freeEnemy;
        }

        private EnemyAi CreateNewEnemy()
        {
            EnemyAi newEnemy = diContainer.InstantiatePrefabForComponent<EnemyAi>(enemyPrefab, enemyContainer);
            enemyPool.Add(newEnemy);

            return newEnemy;
        }
    }
}
