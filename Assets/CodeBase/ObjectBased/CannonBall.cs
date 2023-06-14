using CodeBase.Effects;
using CodeBase.Service;
using CodeBase.Utils;
using System.Collections;
using UnityEngine;
using Zenject;
using static Codebase.Utils.Enums;

namespace CodeBase.ObjectBased
{
    public class CannonBall : ResourceUnit
    {
        #region variables
        [Header("Storages")]
        [SerializeField] private MaterialStorage materialStorage;

        [field: Space]
        [field: SerializeField] public ColorType CurrentColor { get; private set; }
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private int vertexCount = 300;
        [SerializeField] private float radius = 1f;
        [SerializeField] private float randomness = 2f;
        [SerializeField] private float lifeSpan;
        [SerializeField] private int maxRicochets = 3;
        [field: SerializeField] public Rigidbody Rb { get; private set; }

        private Coroutine lifeSpanRoutine;
        private ParticlePool particlePool;
        private int ricochetCounter;
        private ResourcePool resourcePool;
        #endregion

        [Inject]
        private void Construct(ParticlePool pPool, ResourcePool rPool)
        {
            particlePool = pPool;
            resourcePool = rPool;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            ricochetCounter = 0;
            InitBallOnBecomeActive();
        }

        private void InitBallOnBecomeActive()
        {
            Rb.velocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;

            ColorData randomColorData = materialStorage.GetColorData(ColorType.Random);
            meshRenderer.material = randomColorData.Material;
            CurrentColor = randomColorData.Type;
        }

        public override void Take()
        {
            base.Take();
            GenerateRandomVertexForm();
            gameObject.SetActive(true);

            lifeSpanRoutine = StartCoroutine(StartLifeCycle());
        }

        public override void Release()
        {
            base.Release();
            gameObject.SetActive(false);

            StopCoroutine(lifeSpanRoutine);

            particlePool.PlayParticleAction?.Invoke(transform.position, ParticleType.Explosion);
        }

        private void GenerateRandomVertexForm()
        {
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[vertexCount * 3];

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 randomCirclePoint = Random.insideUnitCircle * radius;
                Vector3 randomPoint = new Vector3(randomCirclePoint.x, randomCirclePoint.y, Random.Range(-radius, radius));
                randomPoint += Random.onUnitSphere * randomness;
                vertices[i] = randomPoint.normalized * radius;
                triangles[i * 3] = i;
                triangles[i * 3 + 1] = (i + 1) % vertexCount;
                triangles[i * 3 + 2] = (i + 2) % vertexCount;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
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

        public void Throw(Vector3 force) => Rb.AddForce(Rb.velocity += force / Rb.mass);

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag.Equals(Tags.Enemy))
            {
                Release();
            }
            else
            {
                ricochetCounter++;
                ContactPoint contact = collision.contacts[0];

                if (ricochetCounter == maxRicochets)
                {
                    SpawnMark(contact.point, contact.normal, ResourceType.WallDamage);
                    Release();
                }
                else
                {
                    SpawnMark(contact.point, contact.normal, ResourceType.WallCrack);
                }
            }
        }

        private void SpawnMark(Vector3 position, Vector3 normal, ResourceType markType)
        {
            var newMark = resourcePool.GetFreeResource(markType);
            if (newMark != null)
            {
                newMark.Take();
                newMark.transform.position = position;
                newMark.transform.rotation = Quaternion.LookRotation(normal);

                float randomScale = Random.Range(0.5f, 1f);
                newMark.transform.localScale = new Vector3(randomScale, randomScale, 1f);
            }
        }
    }
}
