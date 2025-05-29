using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_ReadOnlyLoot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // 카드 정보
    [HideInInspector] public S_Skill LootInfo;

    // 카드 애님 관련
    [HideInInspector] public PRS OriginPRS;
    public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;

    // 카드 표시 관련
    [SerializeField] SpriteRenderer sprite_LootBase;

    // 의지 히트나 상점에서 사용
    [HideInInspector] public bool IsSelectedOption; // 제외할 카드 선택 시
    [HideInInspector] public bool IsReadOnly; // 보여주기용 ReadOnly VFX 일 때 사용

    public void SetLootInfo(S_Skill loot)
    {
        // 카드 정보 설정
        LootInfo = loot;

        if (LootInfo.Equals(null)) return;

        // 카드 효과 설정
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{loot.Key}");
        cardEffectOpHandle.Completed += OnLootBaseLoadComplete;
    }
    void OnLootBaseLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_LootBase.sprite = opHandle.Result;
        }
    }
    public void DisappearVFXByExclusion()
    {
        sprite_LootBase.DOFade(0f, S_StackInfoSystem.EXCLUSION_TIME).SetEase(Ease.OutQuart);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.OnDeckInfo || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store)
        {
            GetComponent<Transform>().DOLocalMove(OriginPRS.Pos + new Vector3(0, 0.05f, 0), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            GetComponent<Transform>().DOScale(OriginPRS.Scale + new Vector3(0.12f, 0.12f, 0.12f), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverSkillSystem.Instance.PointerEnterForLootBoardByProduct(LootInfo, transform.position);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Transform>().DOLocalMove(OriginPRS.Pos, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        GetComponent<Transform>().DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverSkillSystem.Instance.PointerExitForSkillObject();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsSelectedOption)
        {
            S_StoreInfoSystem.Instance.CancelSelectLootByOption(LootInfo);
            IsSelectedOption = false;
        }
        else
        {
            S_StoreInfoSystem.Instance.SelectSkillByOption(LootInfo);
            IsSelectedOption = true;
        }
    }
}
