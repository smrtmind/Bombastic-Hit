using CodeBase.Player;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Utils
{
    public class VisibleProjection : MonoBehaviour
    {
        [Header("Storages")]
        [SerializeField] private PlayerStorage playerStorage;

        [Space]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private PlayerController cannonController;
        [SerializeField] private int numPoints = 50;
        [SerializeField] private float timeBetweenPoints = 0.1f;
        [SerializeField] private LayerMask collidableLayers;

        private void Start()
        {
            lineRenderer.enabled = true;
        }

        void Update()
        {
            lineRenderer.positionCount = numPoints;

            List<Vector3> points = new List<Vector3>();
            Vector3 startingPosition = cannonController.ShotPoint.position;
            Vector3 startingVelocity = cannonController.ShotPoint.up * playerStorage.CurrentShootingPower;

            for (float t = 0; t < numPoints; t += timeBetweenPoints)
            {
                Vector3 newPoint = startingPosition + t * startingVelocity;
                newPoint.y = startingPosition.y + startingVelocity.y * t + Physics.gravity.y / 2f * t * t;
                points.Add(newPoint);

                if (Physics.OverlapSphere(newPoint, 2, collidableLayers).Length > 0)
                {
                    lineRenderer.positionCount = points.Count;
                    break;
                }
            }

            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
