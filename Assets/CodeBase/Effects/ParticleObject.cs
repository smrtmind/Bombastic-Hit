using System.Collections;
using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.Effects
{
    public class ParticleObject : MonoBehaviour
    {
        [field: Header("Parameters")]
        [field: SerializeField] public ParticleType ParticleType { get; private set; } = ParticleType.None;

        [Header("Components")]
        [SerializeField] private ParticleSystem particleEffect = default;

        #region get/set
        public bool IsBusy { get; private set; }
        #endregion

        private void Awake()
        {
            particleEffect.gameObject.SetActive(false);
        }

        public void Play()
        {
            StartCoroutine(WaitToDeadParticle());
        }

        private IEnumerator WaitToDeadParticle()
        {
            IsBusy = true;
            particleEffect.gameObject.SetActive(true);
            particleEffect.Play();
            yield return new WaitForSecondsRealtime(particleEffect.main.duration);
            particleEffect.gameObject.SetActive(false);
            IsBusy = false;
        }
    }
}
