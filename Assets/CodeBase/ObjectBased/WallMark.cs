using System.Collections;
using UnityEngine;

namespace CodeBase.ObjectBased
{
    public class WallMark : ResourceUnit
    {
        [SerializeField] private float lifeSpan;

        public override void Take()
        {
            base.Take();
            gameObject.SetActive(true);

            StartCoroutine(StartLifeCycle());
        }

        public override void Release()
        {
            base.Release();
            gameObject.SetActive(false);
        }

        private IEnumerator StartLifeCycle()
        {
            float currentLifeSpan = lifeSpan;

            while (currentLifeSpan > 0f)
            {
                yield return currentLifeSpan -= Time.deltaTime;
            }

            Release();
        }
    }
}
