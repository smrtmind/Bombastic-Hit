using System.Collections.Generic;
using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.ObjectBased
{
    public class Location : MonoBehaviour
    {
        [field: SerializeField] public LocationType LocationType { get; private set; }
        [field: SerializeField] public List<Transform> SpawnPoints { get; private set; }
    }
}
