using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.ObjectBased
{
    public abstract class ResourceUnit : MonoBehaviour
    {
        [field: SerializeField] public ResourceType ResourceType { get; private set; }
        public bool IsBusy { get; private set; }

        public virtual void Take() => IsBusy = true;

        public virtual void Release() => IsBusy = false;
    }
}
