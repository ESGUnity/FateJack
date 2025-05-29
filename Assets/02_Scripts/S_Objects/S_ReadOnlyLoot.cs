using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_ReadOnlyLoot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // ī�� ����
    [HideInInspector] public S_Skill LootInfo;

    // ī�� �ִ� ����
    [HideInInspector] public PRS OriginPRS;
    public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;

    // ī�� ǥ�� ����
    [SerializeField] SpriteRenderer sprite_LootBase;

    // ���� ��Ʈ�� �������� ���
    [HideInInspector] public bool IsSelectedOption; // ������ ī�� ���� ��
    [HideInInspector] public bool IsReadOnly; // �����ֱ�� ReadOnly VFX �� �� ���

    public void SetLootInfo(S_Skill loot)
    {
        // ī�� ���� ����
        LootInfo = loot;

        if (LootInfo.Equals(null)) return;

        // ī�� ȿ�� ����
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
