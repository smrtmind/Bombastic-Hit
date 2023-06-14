using UnityEngine;

namespace CodeBase.Utils
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private int fontSize = 20;
        [SerializeField] private Color fontColor = Color.white;

        private GUIStyle style;
        private Rect position;

        private void Start()
        {
            style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = fontColor;

            position = new Rect(10, 10, 100, 20);
        }

        private void OnGUI()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif

            float fps = 1.0f / Time.deltaTime;
            GUI.Label(position, "FPS: " + fps.ToString("F1"), style);
        }
    }
}
