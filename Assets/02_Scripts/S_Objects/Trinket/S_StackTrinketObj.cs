using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_StackTrinketObj : S_TrinketObj
{
    [SerializeField] Material mat_Origin;
    [SerializeField] Material mat_Dissolve;

    [SerializeField] TMP_Text text_ActivatedCount;

    const float CHANGE_TIME = 0.8f;
    const float MIN_VALUE = 0.05f;
    const float MAX_VALUE = 1f;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Hit, S_GameFlowStateEnum.HittingCard, S_GameFlowStateEnum.Deck, S_GameFlowStateEnum.Store, S_GameFlowStateEnum.StoreBuying };
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            S_HoverInfoSystem.Instance.ActivateHoverInfo(TrinketInfo, sprite_Trinket.gameObject, true);
        }
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
        text_ActivatedCount.GetComponent<MeshRenderer>().sortingOrder = order + 3;
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
        Material newMat = Instantiate(mat_Dissolve);
        sprite_Trinket.material = newMat;

        newMat.SetFloat("_DissolveStrength", MIN_VALUE);

        // 사라짐 (0 -> 1)
        newMat.DOFloat(MAX_VALUE, "_DissolveStrength", CHANGE_TIME)
            .OnComplete(() => Destroy(gameObject));
    }
}
