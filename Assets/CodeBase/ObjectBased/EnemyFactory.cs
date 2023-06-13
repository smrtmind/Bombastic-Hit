using CodeBase.Enemy;
using CodeBase.Player;
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
        [SerializeField] private Transform[] enemySpawnPoints;
        [SerializeField] private int enemiesOnStart;
        [SerializeField] private float spawnDelay;
        [SerializeField] private EnemyAi enemyPrefab;
        [SerializeField] private Transform enemyContainer;

        [Inject] private DiContainer diContainer;

        private List<EnemyAi> enemyPool = new List<EnemyAi>();
        private Coroutine enemySpawnRoutine;

        private void OnEnable()
        {
            UserInterface.OnEnemyTogglePressed += StartSpawner;
        }

        private void OnDisable()
        {
            UserInterface.OnEnemyTogglePressed -= StartSpawner;
        }

        private void Start()
        {
            if (playerStorage.SpawnEnemies)
            {
                BurstSpawn();
                StartSpawner();
            }
        }

        private void BurstSpawn()
        {
            if (enemiesOnStart > 0)
                for (int i = 0; i < enemiesOnStart - 1; i++)
                    SpawnNewEnemy();
        }

        private void StartSpawner() => enemySpawnRoutine = StartCoroutine(SpawnEnemies());

        private IEnumerator SpawnEnemies()
        {
            if (playerStorage.SpawnEnemies)
            {
                while (true)
                {
                    SpawnNewEnemy();
                    yield return new WaitForSeconds(spawnDelay);
                }
            }
            else
            {
                if (enemySpawnRoutine != null)
                    StopCoroutine(enemySpawnRoutine);
            }
        }

        private void SpawnNewEnemy()
        {
            EnemyAi newEnemy = GetFreeEnemy();
            if (newEnemy != null)
            {
                newEnemy.transform.position = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)].transform.position;
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
