using DG.Tweening;
using UnityEngine;

public class S_TweenHelper : MonoBehaviour
{
    const float BOUNCING_SCALE_AMOUNT = 1.25f;

    // 싱글턴
    static S_TweenHelper instance;
    public static S_TweenHelper Instance { get { return instance; } }

    void Awake()
    {
        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BouncingVFX(Transform tf, Vector3 originScale, Vector3 originRot = default)
    {
        Vector3 targetScale = originScale * BOUNCING_SCALE_AMOUNT;

        tf.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(tf.DOScale(targetScale, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        if (originRot != default)
        {
            seq.Join(tf.DOLocalRotate(Vector3.zero, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        }
        seq.Append(tf.DOScale(originScale, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        if (originRot != default)
        {
            seq.Join(tf.DOLocalRotate(originRot, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        }
    }
}
