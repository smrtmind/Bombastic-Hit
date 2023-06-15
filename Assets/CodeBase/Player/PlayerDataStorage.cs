using UnityEngine;

namespace CodeBase.Player
{
    [CreateAssetMenu(fileName = "PlayerDataStorage", menuName = "ScriptableObjects/PlayerDataStorage")]
    public class PlayerDataStorage : ScriptableObject
    {
        [field: Header("Player Data")]
        [field: SerializeField, Min(1)] public int Level { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int Health { get; private set; } = 1;
        [field: SerializeField, Min(0)] public int Score { get; private set; } = 0;
        [field: SerializeField, Range(1f, 28f)] public float ShootingPower { get; private set; } = 28f;
        [field: SerializeField, Min(0.2f)] public float ShootingDelay { get; private set; } = 0.5f;

        [field: Header("World Data")]
        [field: SerializeField] public bool TutorialCompleted { get; private set; } = false;
        [field: SerializeField, Min(0f)] public int ScoreOnKill { get; private set; }
        [field: SerializeField, Range(1, 3)] public int EnemyDamage { get; private set; } = 1;
        [field: SerializeField, Min(5)] public int BaseEnemyAmount { get; private set; } = 5;
        [field: SerializeField, Min(1)] public int EnemyAmountStepPerLevel { get; private set; } = 2;
        [field: SerializeField, Min(1f)] public float EnemySpawnDelay { get; private set; } = 1f;
    }
}
