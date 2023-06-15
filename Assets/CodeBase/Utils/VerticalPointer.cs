using DG.Tweening;
using UnityEngine;

namespace CodeBase.Utils
{
    public class VerticalPointer : MonoBehaviour
    {
        [SerializeField] private float minY;
        [SerializeField] private float maxY;
        [SerializeField] private float speedOfTransition;

        private Sequence moveArrowSequence;
        private Tween rotateArrowTween;

        private void OnEnable()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, minY, transform.localPosition.z);

            rotateArrowTween?.Kill();
            rotateArrowTween = transform.DORotate(new Vector3(0f, 360f, 0f), speedOfTransition * 2f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            moveArrowSequence = DOTween.Sequence();
            moveArrowSequence.Append(transform.DOLocalMoveY(maxY, speedOfTransition).SetEase(Ease.Linear))
                    .Append(transform.DOLocalMoveY(minY, speedOfTransition).SetEase(Ease.Linear))
                    .SetLoops(-1);
        }

        private void OnDisable()
        {
            rotateArrowTween?.Kill();
            moveArrowSequence?.Kill();
        }
    }
}
