using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

// S_SkillInfoSystem에 들어가는 실질적인 능력 프리팹
public class S_UISkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("능력 정보")]
    [HideInInspector] public S_Skill SkillInfo;

    [Header("컴포넌트")]
    Image image_IsMeetConditionEffect;
    Image image_Skill;
    TMP_Text text_ActivatedCount;

    [Header("연출 관련")]
    Vector3 originScale;

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // 컴포넌트 할당
        image_IsMeetConditionEffect = Array.Find(images, c => c.gameObject.name.Equals("Image_IsMeetConditionEffect"));
        image_Skill = Array.Find(images, c => c.gameObject.name.Equals("Image_Skill"));
        text_ActivatedCount = Array.Find(texts, c => c.gameObject.name.Equals("Text_ActivatedCount"));

        originScale = transform.localScale;
    }

    #region 포인터 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit || 
            S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.HittingCard || 
            S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store || 
            S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Dialog)
        {
            S_HoverInfoSystem.Instance.ActivateHoverInfo(SkillInfo, GetComponent<RectTransform>());
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        S_HoverInfoSystem.Instance.DeactiveHoverInfo();
    }
    #endregion
    #region 주요 함수
    public void SetSkillObjectInfo(S_Skill skill)
    {
        SkillInfo = skill;

        if (SkillInfo.Passive == S_SkillPassiveEnum.NeedActivatedCount)
        {
            text_ActivatedCount.gameObject.SetActive(true);
            text_ActivatedCount.text = SkillInfo.ActivatedCount.ToString();
        }
        else
        {
            text_ActivatedCount.gameObject.SetActive(false);
        }

        // 이미지 세팅
        var skillOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{SkillInfo.Key}");
        skillOpHandle.Completed += OnSkillSpriteLoadComplete;
    }
    void OnSkillSpriteLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_Skill.sprite = opHandle.Result;
        }
    }
    public void UpdateSkillObject() // 능력 오브젝트 업데이트
    {
        if (SkillInfo.Passive == S_SkillPassiveEnum.NeedActivatedCount)
        {
            ChangeCountVFXTween(int.Parse(text_ActivatedCount.text), SkillInfo.ActivatedCount, text_ActivatedCount);
        }
        else
        {
            text_ActivatedCount.gameObject.SetActive(false);
        }

        if (SkillInfo.IsMeetCondition)
        {
            image_IsMeetConditionEffect.gameObject.SetActive(true);
        }
        else
        {
            image_IsMeetConditionEffect.gameObject.SetActive(false);
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
    #endregion
    public void BouncingVFX()
    {
        S_TweenHelper.Instance.BouncingVFX(transform, originScale);
    }
}
