using CodeBase.Effects;
using CodeBase.Utils;
using System.Collections;
using UnityEngine;
using Zenject;
using static Codebase.Utils.Enums;

namespace CodeBase.ObjectBased
{
    public class CannonBall : ResourceUnit, IAmThrowable
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private int numVertices = 10;
        [SerializeField] private float minRange = -5f;
        [SerializeField] private float maxRange = 5f;
        [SerializeField] private float lifeSpan;
        [field: SerializeField] public PhysicsObject PhysicsObject { get; private set; }

        private Coroutine lifeSpanRoutine;
        private ParticlePool particlePool;

        [Inject]
        private void Construct(ParticlePool pPool)
        {
            particlePool = pPool;
        }

        private void OnEnable()
        {
            PhysicsObject.OnReachedMaxRicochets += Release;
        }

        private void OnDisable()
        {
            PhysicsObject.OnReachedMaxRicochets -= Release;
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

        public void Throw(Vector3 force) => PhysicsObject.AddForce(force);
    }
}
