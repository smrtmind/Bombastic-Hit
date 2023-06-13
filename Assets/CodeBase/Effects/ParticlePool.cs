using System;
using System.Collections.Generic;
using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.Effects
{
    public class ParticlePool : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform objectsContainer;
        [SerializeField] private List<ParticleObject> objects = new List<ParticleObject>();

        #region Action
        public Action<Vector3, ParticleType> PlayParticleAction = default;
        #endregion

        #region Variables
        private List<ParticleObject> particleObjects = new List<ParticleObject>();
        #endregion

        private void Awake()
        {
            PrepareParticles();
        }

        private void OnEnable()
        {
            PlayParticleAction += PlayParticle;
        }

        private void OnDisable()
        {
            PlayParticleAction -= PlayParticle;
        }

        private void PlayParticle(Vector3 position, ParticleType particleType)
        {
            ParticleObject particle = GetFreeParticle(particleType);
            particle.transform.position = position;
            particle.Play();
        }

        private ParticleObject GetFreeParticle(ParticleType particleType)
        {
            ParticleObject result = particleObjects.Find((part) => part.ParticleType == particleType && !part.IsBusy);
            if (result == null)
            {
                result = CreateParticle(particleType);
            }

            return result;
        }

        private void PrepareParticles()
        {
            foreach (var particle in particleObjects)
            {
                for (int i = 0; i < 10; i++)
                {
                    CreateParticle(particle.ParticleType);
                }
            }
        }

        private ParticleObject CreateParticle(ParticleType particleType)
        {
            ParticleObject result = Instantiate(objects.Find((_prefab) => _prefab.ParticleType == particleType).gameObject, objectsContainer).GetComponent<ParticleObject>();
            particleObjects.Add(result);
            return result;
        }
    }
}
