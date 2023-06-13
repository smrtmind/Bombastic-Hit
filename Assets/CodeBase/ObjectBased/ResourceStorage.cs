using System.Collections.Generic;
using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.ObjectBased
{
    [CreateAssetMenu(fileName = "ResourceStorage", menuName = "ScriptableObjects/ResourceStorage")]
    public class ResourceStorage : ScriptableObject
    {
        [SerializeField] private List<ResourceUnit> resourceUnits;

        public ResourceUnit GetResourceUnit(ResourceType type)
        {
            foreach (ResourceUnit resource in resourceUnits)
            {
                if (resource.ResourceType == type)
                    return resource;
            }

            return null;
        }
    }
}
