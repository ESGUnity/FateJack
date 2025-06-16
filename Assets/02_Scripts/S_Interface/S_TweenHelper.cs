using DG.Tweening;
using TMPro;
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

    Sequence bouncingSeq;
    public void BouncingVFX(Transform tf, Vector3 originScale, Vector3 originRot = default)
    {
        Vector3 targetScale = originScale * BOUNCING_SCALE_AMOUNT;

        tf.DOKill();
        bouncingSeq.Kill();

        bouncingSeq = DOTween.Sequence();

        bouncingSeq.Append(tf.DOScale(targetScale, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        if (originRot != default)
        {
            bouncingSeq.Join(tf.DOLocalRotate(Vector3.zero, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        }
        bouncingSeq.Append(tf.DOScale(originScale, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        if (originRot != default)
        {
            bouncingSeq.Join(tf.DOLocalRotate(originRot, S_EffectActivator.Instance.GetEffectLifeTime() / 5).SetEase(Ease.OutQuad));
        }
    }

    Tween changeValueTween;
    public void ChangeValueVFX(int oldValue, int newValue, TMP_Text statText)
    {
        // 기존 트윈 종료
        if (changeValueTween != null && changeValueTween.IsActive() && changeValueTween.IsPlaying())
        {
            changeValueTween.Kill();
        }

        int currentNumber = oldValue;

        // 트윈 시작
        changeValueTween = DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = currentNumber.ToString(); },
                newValue,
                S_EffectActivator.Instance.GetEffectLifeTime() * 0.9f
            ).SetEase(Ease.OutQuart);
    }
}
