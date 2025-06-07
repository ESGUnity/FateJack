using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

// 상점에서만 쓰이는 오브젝트형 능력(원래 능력은 UI다)
public class S_SkillObject : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public S_Skill SkillInfo;

    [Header("컴포넌트")]
    [SerializeField] SpriteRenderer sprite_Skill;
    [SerializeField] SpriteRenderer sprite_DecideBtn;
    [SerializeField] TMP_Text text_Decide;

    [Header("VFX")]
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;
    public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;
    const float POINTER_ENTER_SCALE_AMOUNT = 1.2f;

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);

        // 컴포넌트 할당
        sprite_Skill = System.Array.Find(sprites, c => c.gameObject.name.Equals("Sprite_Skill"));
        sprite_DecideBtn = System.Array.Find(sprites, c => c.gameObject.name.Equals("Sprite_DecideBtn"));
        text_Decide = System.Array.Find(texts, c => c.gameObject.name.Equals("Text_Decide"));
    }

    #region 초기화
    public void SetSkillInfo(S_Skill skill)
    {
        SkillInfo = skill;

        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{skill.Key}");
        cardEffectOpHandle.Completed += OnSkillSpriteLoadComplete;

        // 소팅오더 설정
        SetOrder(1);
    }
    void OnSkillSpriteLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_Skill.sprite = opHandle.Result;
        }
    }
    public void SetOrder(int order) // 상품의 오더 설정
    {
        sprite_Skill.sortingLayerName = "WorldObject";
        sprite_Skill.sortingOrder = order;

        sprite_DecideBtn.sortingLayerName = "WorldObject";
        sprite_DecideBtn.sortingOrder = order + 1;

        text_Decide.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_Decide.GetComponent<MeshRenderer>().sortingOrder = order + 2;
    }
    public void SetAlphaValue(float value, float duration)
    {
        sprite_Skill.DOFade(value, duration);
        sprite_DecideBtn.DOFade(value, duration);
        text_Decide.DOFade(value, duration);
    }
    #endregion
    #region 포인터 함수
    public void PointerEnterOnSkillSprite()
    {
        sprite_Skill.transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverInfoSystem.Instance.ActivateHoverInfo(SkillInfo, sprite_Skill.gameObject);
    }
    public void PointerExitOnSkillSprite()
    {
        sprite_Skill.transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();
    }
    public void ClickDecideBtn()
    {
        S_StoreInfoSystem.Instance.DecideSkillOption(SkillInfo);

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();

        Destroy(gameObject);
    }
    #endregion
}
