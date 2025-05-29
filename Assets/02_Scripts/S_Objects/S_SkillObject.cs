using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_SkillObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("능력 정보")]
    [HideInInspector] public S_Skill SkillInfo;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject sprite_IsMeetConditionEffect;

    [Header("컴포넌트")]
    Image image_Icon;
    TMP_Text text_Count;

    [Header("연출 관련")]
    Vector3 originScale;
    const float BOUNCING_SCALE_AMOUNT = 1.25f;

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // 컴포넌트 할당
        image_Icon = Array.Find(images, c => c.gameObject.name.Equals("Image_Icon"));
        text_Count = Array.Find(texts, c => c.gameObject.name.Equals("Text_Count"));

        originScale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit || 
            S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.HittingCard || 
            S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store || 
            S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Dialog)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, GetComponent<RectTransform>().position);
            S_HoverSkillSystem.Instance.PointerEnterForSkillObject(SkillInfo, screenPos);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        S_HoverSkillSystem.Instance.PointerExitForSkillObject();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.StoreByRemove)
        {
            S_StoreInfoSystem.Instance.SelectSkillByOption(SkillInfo);
        }
    }

    public void SetSkillObjectInfo(S_Skill skill) // 초기화
    {
        SkillInfo = skill;

        if (SkillInfo.Passive == S_SkillPassiveEnum.NeedActivatedCount)
        {
            text_Count.gameObject.SetActive(true);
            text_Count.text = SkillInfo.ActivatedCount.ToString();
        }
        else
        {
            text_Count.gameObject.SetActive(false);
        }

        // 이미지 세팅
        var skillOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{SkillInfo.Key}");
        skillOpHandle.Completed += OnSkillSpriteLoadComplete;
    }
    void OnSkillSpriteLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_Icon.sprite = opHandle.Result;
        }
    }

    public void UpdateSkillObject() // 능력 오브젝트 업데이트
    {
        if (SkillInfo.Passive == S_SkillPassiveEnum.NeedActivatedCount)
        {
            ChangeCountVFXTween(int.Parse(text_Count.text), SkillInfo.ActivatedCount, text_Count);
        }
        else
        {
            text_Count.gameObject.SetActive(false);
        }

        if (SkillInfo.CanActivateEffect)
        {
            sprite_IsMeetConditionEffect.SetActive(true);
        }
        else
        {
            sprite_IsMeetConditionEffect.SetActive(false);
        }
    }
    void ChangeCountVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = currentNumber.ToString(); },
                newValue,
                S_EffectActivator.Instance.GetEffectLifeTime() * 0.8f
            ).SetEase(Ease.OutQuart);
    }

    public void BouncingVFX()
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(originScale * BOUNCING_SCALE_AMOUNT, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originScale, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad));
    }
}
