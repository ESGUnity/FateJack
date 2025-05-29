using DG.Tweening;
using UnityEngine;

public class GetGoldVFX : MonoBehaviour
{
    float liftTime = 0.5f;

    public void TriggerVFX()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOMove(transform.position - new Vector3(0, 20f, 0), liftTime))
            .Join(transform.DORotate(new Vector3(0, 360f * 5f, 0), liftTime))
            .OnComplete(() => Destroy(gameObject));
    }
}
