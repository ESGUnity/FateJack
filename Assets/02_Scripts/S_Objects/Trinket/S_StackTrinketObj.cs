using DG.Tweening;
using TMPro;
using UnityEngine;

public class S_StackTrinketObj : S_TrinketObj
{
    [SerializeField] Material m_Origin;
    [SerializeField] Material m_Dissolve;

    [SerializeField] TMP_Text text_ActivatedCount;

    const float CHANGE_TIME = 0.8f;
    const float MIN_VALUE = 0.05f;
    const float MAX_VALUE = 1f;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Hit, S_GameFlowStateEnum.HittingCard, S_GameFlowStateEnum.Store, S_GameFlowStateEnum.StoreBuying, S_GameFlowStateEnum.Dialog };
    }

    public override void SetTrinketInfo(S_Trinket trinket)
    {
        base.SetTrinketInfo(trinket);

        if (TrinketInfo.IsNeedActivatedCount)
        {
            text_ActivatedCount.gameObject.SetActive(true);
            text_ActivatedCount.text = TrinketInfo.ActivatedCount.ToString();
        }
        else
        {
            text_ActivatedCount.gameObject.SetActive(false);
        }
    }
    public override void SetOrder(int order)
    {
        base.SetOrder(order);

        text_ActivatedCount.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_ActivatedCount.GetComponent<MeshRenderer>().sortingOrder = order + 2;
    }
    public override void UpdateTrinketObj()
    {
        base.UpdateTrinketObj();

        // ActivatedCount 업데이트
        if (TrinketInfo.IsNeedActivatedCount)
        {
            S_TweenHelper.Instance.ChangeValueVFX(int.Parse(text_ActivatedCount.text), TrinketInfo.ActivatedCount, text_ActivatedCount);
        }
        else
        {
            text_ActivatedCount.gameObject.SetActive(false);
        }
    }
    public override void SetAlphaValue(float value, float duration)
    {
        base.SetAlphaValue(value, duration);

        text_ActivatedCount.DOKill();
        text_ActivatedCount.DOFade(value, duration);
    }

    public void DeleteTrinketVFX()
    {
        Material newMat = Instantiate(m_Dissolve);
        sprite_Trinket.material = newMat;

        newMat.SetFloat("_DissolveStrength", MIN_VALUE);

        // 사라짐 (0 -> 1)
        newMat.DOFloat(MAX_VALUE, "_DissolveStrength", CHANGE_TIME)
            .OnComplete(() => Destroy(gameObject));
    }
}
