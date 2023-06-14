using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Player
{
    [CreateAssetMenu(fileName = "PlayerStorage", menuName = "ScriptableObjects/PlayerStorage")]
    public class PlayerStorage : ScriptableObject
    {
        [SerializeField] private string playerPrefsSaveString = "_playerSave";

        [Header("Data Storage")]
        [SerializeField] private PlayerDataStorage playerDataStorage;

        [Header("ConcretePlayer")]
        [SerializeField] private Player playerData = new Player();

        public Player PlayerData => playerData;

        public void SavePlayer()
        {
            Debug.Log("SAVED");
            PlayerPrefs.SetString(playerPrefsSaveString, JsonUtility.ToJson(playerData));
        }

        public void LoadPlayer()
        {
            var playerString = PlayerPrefs.GetString(playerPrefsSaveString, "");
            if (playerString != "")
            {
                Debug.Log("LOADED");
                playerData = JsonUtility.FromJson<Player>(playerString);

                if (!playerData.TutorialCompleted)
                    CreateNewSave();
            }
            else
            {
                CreateNewSave();
            }
        }

        private void CreateNewSave()
        {
            Debug.Log("NEW GAME");
            playerData = new Player();
            InitPlayer();
            //InitUpgrades();
        }

        private void InitPlayer()
        {
            playerData.SetPlayerData(playerDataStorage.Level,
                                     playerDataStorage.Health,
                                     playerDataStorage.Score,
                                     playerDataStorage.ShootingPower,
                                     playerDataStorage.TutorialCompleted,
                                     playerDataStorage.ScoreOnKill,
                                     playerDataStorage.EnemyDamage,
                                     playerDataStorage.BaseEnemyAmount,
                                     playerDataStorage.EnemyAmountStepPerLevel);
        }

        //private void InitUpgrades()
        //{
        //    List<ObjectType> objectTypes = new List<ObjectType>();
        //    foreach (var data in playerDataStorage.UpgradesDatas)
        //    {
        //        objectTypes.Add(data.ObjectType);
        //    }

        //    for (int i = 0; i < objectTypes.Count; i++)
        //    {
        //        var tech = playerDataStorage.GetUpgradeData(objectTypes[i]);
        //        Technology newTech = new Technology(tech.ObjectType,
        //                                            tech.ObjectCost);

        //        playerData.ObjectsDatas.Add(newTech);
        //    }
        //}
    }
}
