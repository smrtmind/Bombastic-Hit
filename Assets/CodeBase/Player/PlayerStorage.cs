using UnityEngine;

namespace CodeBase.Player
{
    [CreateAssetMenu(fileName = "PlayerStorage", menuName = "ScriptableObjects/PlayerStorage")]
    public class PlayerStorage : ScriptableObject
    {
        [field: Header("Player Settings")]
        [field: SerializeField, Min(0f)] public float Score { get; private set; }
        [field: SerializeField, Range(1f, 100f)] public float MaxShootingPower { get; private set; }
        [field: SerializeField, Min(0f)] public float CurrentShootingPower { get; private set; }

        [field: Header("Enemy Setting")]
        [field: SerializeField] public bool SpawnEnemies { get; private set; }
        [field: SerializeField, Min(0f)] public float ScoreOnKill { get; private set; }
        [field: SerializeField, Min(0f)] public float ScorePenalty { get; private set; }

        public void ModifyScore(float value)
        {
            Score += value;
            if (Score < 0f)
                Score = 0f;
        }

        public void ModifyShootingPowerValue(float value) => CurrentShootingPower = value;

        public void ModifyEnemySpawner(bool enable) => SpawnEnemies = enable;
    }
}
