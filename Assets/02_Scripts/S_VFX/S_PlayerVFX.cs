using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_PlayerVFX : MonoBehaviour
{
    public async Task VFXAsync(S_PlayerVFXEnum vfx, Vector2 pos)
    {
        // ��������Ʈ �ҷ�����
        var opHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_PlayerVFX_{vfx.ToString()}");
        opHandle.Completed += OnVFXEffectLoadComplete;

        // VFX ���� �ʱ�ȭ
        GetComponent<RectTransform>().anchoredPosition = pos;
        transform.localScale = Vector3.zero;
        GetComponent<Image>().DOFade(0, 0);

        // �ִ� Ʈ��
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(Vector3.one, S_EffectActivator.Instance.GetEffectLifeTime())).SetEase(Ease.OutQuad)
            .Join(GetComponent<Image>().DOFade(0.8f, S_EffectActivator.Instance.GetEffectLifeTime() / 3).SetEase(Ease.OutQuad))
            .AppendInterval(S_EffectActivator.Instance.GetEffectLifeTime() / 3)
            .Append(GetComponent<Image>().DOFade(0f, S_EffectActivator.Instance.GetEffectLifeTime() / 3).SetEase(Ease.OutQuad))
            .OnComplete(() => Destroy(gameObject));

        await seq.AsyncWaitForCompletion();
    }

    void OnVFXEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GetComponent<Image>().sprite = opHandle.Result;
        }
    }
}
