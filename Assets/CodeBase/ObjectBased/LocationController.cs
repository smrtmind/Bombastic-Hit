using CodeBase.Player;
using CodeBase.UI;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.ObjectBased
{
    public class LocationController : MonoBehaviour
    {
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;

        [Space]
        [SerializeField] private List<Location> locations;

        private void Start()
        {
            RebuildLocations();
        }

        private void OnEnable()
        {
            GamePanelController.OnLevelChanged += RebuildLocations;
        }

        private void OnDisable()
        {
            GamePanelController.OnLevelChanged -= RebuildLocations;
        }

        public List<Transform> GetCurrentSpawnPoints()
        {
            List<Transform> result = new List<Transform>();

            foreach (var location in locations)
            {
                if (location.LocationType == playerStorage.PlayerData.CurrentLocationType)
                {
                    foreach (var spawnPoint in location.SpawnPoints)
                        result.Add(spawnPoint);

                    break;
                }
            }

            return result;
        }

        private void RebuildLocations()
        {
            locations.ForEach(location => location.gameObject.SetActive(false));

            foreach (var location in locations)
            {
                if (location.LocationType == playerStorage.PlayerData.CurrentLocationType)
                    location.gameObject.SetActive(true);
            }
        }
    }
}
