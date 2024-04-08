using DG.Tweening;
using UnityEngine;

namespace _projectserhojgi.TXTFILES
{
    public class Shake : MonoBehaviour
    {
        private void Start()
        {
            Sequence rotate = DOTween.Sequence();
            rotate.Append(
                transform.DORotate(Vector3.forward * 15, 0.3f).SetEase(Ease.Linear));
            rotate.Append(
                transform.DORotate(Vector3.forward * 0, 0.3f).SetEase(Ease.Linear));
            rotate.Append(
                transform.DORotate(Vector3.forward * -15, 0.3f).SetEase(Ease.Linear));
            rotate.Append(
                transform.DORotate(Vector3.forward * 0, 0.3f).SetEase(Ease.Linear));
            rotate.SetLoops(-1);
        }
    }
}