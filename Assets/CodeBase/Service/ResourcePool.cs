using CodeBase.ObjectBased;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Codebase.Utils.Enums;

namespace CodeBase.Service
{
    public class ResourcePool : MonoBehaviour
    {
        [Header("Storages")]
        [SerializeField] private ResourceStorage resourceStorage;

        [Space]
        [SerializeField] private Transform resourceContainer;

        [Inject] private DiContainer diContainer;

        private List<ResourceUnit> resourcePool = new List<ResourceUnit>();

        public ResourceUnit GetFreeResource(ResourceType type)
        {
            ResourceUnit freeResource = resourcePool.Find(resource => !resource.IsBusy && resource.ResourceType == type);
            if (freeResource == null)
                freeResource = CreateNewResource(type);

            return freeResource;
        }

        private ResourceUnit CreateNewResource(ResourceType type)
        {
            ResourceUnit newResource = diContainer.InstantiatePrefabForComponent<ResourceUnit>(resourceStorage.GetResourceUnit(type), resourceContainer);
            resourcePool.Add(newResource);

            return newResource;
        }
    }
}
