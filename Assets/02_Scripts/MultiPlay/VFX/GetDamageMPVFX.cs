using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GetDamageMPVFX : MonoBehaviour
{
    float liftTime = 2f;

    public void TriggerVFX()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOMove(transform.position + new Vector3(0, 1f, 0), liftTime))
            .Join(GetComponent<SpriteRenderer>().DOFade(0, liftTime).SetEase(Ease.InQuad))
            .OnComplete(() => Destroy(gameObject));
    }
}
