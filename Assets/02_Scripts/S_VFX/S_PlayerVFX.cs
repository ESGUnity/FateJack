using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_PlayerVFX : MonoBehaviour
{
    SpriteRenderer sprite_PlayerVFX;

    //public async Task VFXAsync(S_PlayerVFXEnum vfx, Vector3 pos)
    //{
    //    sprite_PlayerVFX = GetComponent<SpriteRenderer>();

    //    // 스프라이트 불러오기
    //    var opHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_PlayerVFX_{vfx.ToString()}");
    //    opHandle.Completed += OnVFXEffectLoadComplete;

    //    // VFX 각종 초기화
    //    transform.position = pos;
    //    transform.localScale = Vector3.zero;
    //    sprite_PlayerVFX.DOFade(0, 0);

    //    // 애님 트윈
    //    Sequence seq = DOTween.Sequence();

    //    seq.Append(transform.DOScale(Vector3.one, S_EffectActivator.Instance.GetEffectLifeTime())).SetEase(Ease.OutQuad)
    //        .Join(sprite_PlayerVFX.DOFade(0.8f, S_EffectActivator.Instance.GetEffectLifeTime() / 3).SetEase(Ease.OutQuad))
    //        .AppendInterval(S_EffectActivator.Instance.GetEffectLifeTime() / 3)
    //        .Append(sprite_PlayerVFX.DOFade(0f, S_EffectActivator.Instance.GetEffectLifeTime() / 3).SetEase(Ease.OutQuad))
    //        .OnComplete(() => Destroy(gameObject));

    //    await seq.AsyncWaitForCompletion();
    //}
    public async Task VFXAsync(S_PlayerVFXEnum vfx, GameObject go)
    {
        transform.SetParent(go.transform);
        sprite_PlayerVFX = GetComponent<SpriteRenderer>();

        // 스프라이트 불러오기
        var opHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_PlayerVFX_{vfx.ToString()}");
        opHandle.Completed += OnVFXEffectLoadComplete;

        // VFX 각종 초기화
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.zero;
        sprite_PlayerVFX.DOFade(0, 0);

        // 애님 트윈
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(Vector3.one, S_EffectActivator.Instance.GetEffectLifeTime())).SetEase(Ease.OutQuad)
            .Join(sprite_PlayerVFX.DOFade(0.8f, S_EffectActivator.Instance.GetEffectLifeTime() / 3).SetEase(Ease.OutQuad))
            .AppendInterval(S_EffectActivator.Instance.GetEffectLifeTime() / 3)
            .Append(sprite_PlayerVFX.DOFade(0f, S_EffectActivator.Instance.GetEffectLifeTime() / 3).SetEase(Ease.OutQuad))
            .OnComplete(() => Destroy(gameObject));

        await seq.AsyncWaitForCompletion();
    }
    void OnVFXEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_PlayerVFX.sprite = opHandle.Result;
        }
    }
}
