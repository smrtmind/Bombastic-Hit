using CodeBase.UI;
using System;
using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.Player
{
    [Serializable]
    public class Player
    {
        [field: Header("Player Settings")]
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public int CurrentHealth { get; private set; }
        [field: SerializeField] public int Score { get; private set; }
        [field: SerializeField] public float ShootingPower { get; private set; }
        [field: SerializeField] public float ShootingDelay { get; private set; }

        [field: Header("World Settings")]
        [field: SerializeField] public bool TutorialCompleted { get; private set; }
        [field: SerializeField] public LocationType CurrentLocationType { get; private set; }
        [field: SerializeField] public int ScoreOnKill { get; private set; }
        [field: SerializeField] public int EnemyDamage { get; private set; }
        [field: SerializeField] public int EnemyAmount { get; private set; }
        [field: SerializeField] public int CurrentEnemyAmount { get; private set; }
        [field: SerializeField] public int EnemyAmountStepPerLevel { get; private set; }
        [field: SerializeField] public float EnemySpawnDelay { get; private set; }

        public void SetPlayerData(int level, int health, int score, float shootingPower, float shootingDelay, bool tutorialCompleted, int scoreOnKill, int enemyDamage, int baseEnemyAmount, int enemyAmountStepPerLevel, float enemySpawnDelay)
        {
            Level = level;
            Health = health;
            Score = score;
            ShootingPower = shootingPower;
            ShootingDelay = shootingDelay;
            TutorialCompleted = tutorialCompleted;
            ScoreOnKill = scoreOnKill;
            EnemyDamage = enemyDamage;
            EnemyAmount = baseEnemyAmount;
            EnemyAmountStepPerLevel = enemyAmountStepPerLevel;
            EnemySpawnDelay = enemySpawnDelay;

            CurrentLocationType = LocationType.Single;

            CurrentHealth = health;
        }

        public void ModifyScore()
        {
            Score += ScoreOnKill;
            if (Score < 0)
                Score = 0;

            GamePanelController.OnScoreChanged?.Invoke();
        }

        public void ModifyHealth()
        {
            CurrentHealth -= EnemyDamage;
            if (CurrentHealth <= 0)
            {
                LoadingScreen.OnLevelEnded?.Invoke($"you{Environment.NewLine}lose", Color.red);
                CurrentHealth = Health;
                CurrentEnemyAmount = 0;

                GamePanelController.OnLevelProgressChanged?.Invoke();
            }

            GamePanelController.OnHealthChanged?.Invoke();
        }

        public void ModifyEnemyAmount()
        {
            CurrentEnemyAmount++;

            if (CurrentEnemyAmount >= EnemyAmount)
            {
                LoadingScreen.OnLevelEnded?.Invoke($"level {Level}{Environment.NewLine}complete", Color.green);

                Level++;
                CurrentEnemyAmount = 0;

                switch (Level)
                {
                    case 1:
                    case 2:
                        TutorialCompleted = true;

                        EnemyAmount *= 2;
                        CurrentLocationType = LocationType.Double;
                        break;

                    default:
                        EnemyAmount += EnemyAmountStepPerLevel;
                        CurrentLocationType = LocationType.Triple;
                        break;
                }

                GamePanelController.OnLevelChanged?.Invoke();
            }

            GamePanelController.OnLevelProgressChanged?.Invoke();
        }
    }
}
