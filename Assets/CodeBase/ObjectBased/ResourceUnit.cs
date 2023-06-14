using CodeBase.UI;
using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.ObjectBased
{
    public abstract class ResourceUnit : MonoBehaviour
    {
        [field: SerializeField] public ResourceType ResourceType { get; private set; }
        public bool IsBusy { get; private set; }

        protected virtual void OnEnable()
        {
            LoadingScreen.OnLoadingScreenActive += Release;
        }

        private void OnDisable()
        {
            LoadingScreen.OnLoadingScreenActive -= Release;
        }

        public virtual void Take() => IsBusy = true;

        public virtual void Release() => IsBusy = false;
    }
}
