using CodeBase.Player;
using UnityEngine;
using Zenject;

namespace CodeBase.Service
{
    public class CameraController : MonoBehaviour
    {
        [field: SerializeField] public Camera MainCamera { get; private set; }

        private Transform playerTransform;

        [Inject]
        private void Construct(PlayerController player)
        {
            playerTransform = player.transform;
        }

        private void Awake()
        {
            transform.SetParent(playerTransform);
        }
    }
}
