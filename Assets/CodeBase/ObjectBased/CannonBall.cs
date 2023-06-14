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
        [SerializeField] private int numVertices = 10;
        [SerializeField] private float minRange = -5f;
        [SerializeField] private float maxRange = 5f;
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

        private void OnEnable()
        {
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

            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] uvs = new Vector2[numVertices];

            float radius = (maxRange - minRange) / 2f;
            Vector3 center = Vector3.zero;

            for (int i = 0; i < numVertices; i++)
            {
                float theta = Random.Range(0f, Mathf.PI * 2f);
                float phi = Random.Range(0f, Mathf.PI);

                float x = center.x + radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = center.y + radius * Mathf.Sin(phi) * Mathf.Sin(theta);
                float z = center.z + radius * Mathf.Cos(phi);

                vertices[i] = new Vector3(x, y, z);
                uvs[i] = new Vector2(theta / (Mathf.PI * 2f), phi / Mathf.PI);
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;

            int[] triangles = GenerateTriangles(numVertices);
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private int[] GenerateTriangles(int vertexCount)
        {
            int triangleCount = vertexCount - 2;
            int[] triangles = new int[triangleCount * 3];

            int currentIndex = 0;
            for (int i = 0; i < triangleCount; i++)
            {
                triangles[currentIndex] = 0;
                triangles[currentIndex + 1] = i + 1;
                triangles[currentIndex + 2] = i + 2;

                currentIndex += 3;
            }

            return triangles;
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
