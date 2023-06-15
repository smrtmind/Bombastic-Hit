using CodeBase.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.Utils
{
    public class TutorialInfo : MonoBehaviour
    {
        [SerializeField] private Canvas infoBoxCanvas;
        [SerializeField] private TextMeshProUGUI messageValue;

        private Coroutine canvasRoutine;
        private Camera mainCamera;

        [Inject]
        private void Construct(CameraController cameraController)
        {
            mainCamera = cameraController.MainCamera;
        }

        private void OnEnable()
        {
            canvasRoutine = StartCoroutine(SpecialCoroutine.LookAt(infoBoxCanvas.transform, mainCamera.transform));
        }

        private void OnDisable()
        {
            if (canvasRoutine != null)
            {
                StopCoroutine(canvasRoutine);
                canvasRoutine = null;
            }
        }

        public void SetText(string text) => messageValue.text = text;
    }
}
