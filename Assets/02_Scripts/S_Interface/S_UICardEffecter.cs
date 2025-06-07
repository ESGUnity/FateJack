using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

// 덱 카드 저주, 제외, 카드 획득, 카드 소멸 등 UICard를 사용하는 모든 VFX
public class S_UICardEffecter : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_UICard;
    [SerializeField] GameObject prefab_ExpansionUICard;

    [Header("컴포넌트")]
    GameObject layoutGroup_ExpansionCards;
    GameObject image_HideExpansionCardsBtn;
    TMP_Text text_HideExpansionCardsBtn;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject sprite_BlackBackgroundByExpansionCards;
    [SerializeField] GameObject sprite_WorldObjectBlockingBackground;


    [Header("애님 관련")]
    Vector2 curseDeckCardStartPos = new Vector2(0, -150);
    Vector2 curseDeckCardEndPos = new Vector2(0, 50);
    Vector2 exclusionDeckCardStartPos = new Vector2(0, -300);
    Vector2 exclusionDeckCardEndPos = new Vector2(0, 150);
    float SHOW_EXPANSION_TIME = 0.2f;
    bool isShowExpansionCards = false;

    // 싱글턴
    static S_UICardEffecter instance;
    public static S_UICardEffecter Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // 히트 버튼 관련 컴포넌트 할당
        layoutGroup_ExpansionCards = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_ExpansionCards")).gameObject;
        image_HideExpansionCardsBtn = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HideExpansionCardsBtn")).gameObject;
        text_HideExpansionCardsBtn = Array.Find(texts, c => c.gameObject.name.Equals("Text_HideExpansionCardsBtn"));

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
    #region 연출
    public void AppearBlockingPanel()
    {
        sprite_BlackBackgroundByExpansionCards.SetActive(true);
        sprite_WorldObjectBlockingBackground.SetActive(true);
        sprite_BlackBackgroundByExpansionCards.GetComponent<SpriteRenderer>().DOKill();
        sprite_BlackBackgroundByExpansionCards.GetComponent<SpriteRenderer>().DOFade(0.8f, SHOW_EXPANSION_TIME);
    }
    public void DisappearBlockingPanel()
    {
        sprite_BlackBackgroundByExpansionCards.GetComponent<SpriteRenderer>().DOKill();
        sprite_BlackBackgroundByExpansionCards.GetComponent<SpriteRenderer>().DOFade(0, SHOW_EXPANSION_TIME)
            .OnComplete(() =>
            {
                sprite_BlackBackgroundByExpansionCards.SetActive(false);
                sprite_WorldObjectBlockingBackground.SetActive(false);
            });
    }
    #endregion
    #region 각종 UICard 효과
    public void CurseDeckCardVFX(S_Card card)
    {
        GameObject go = Instantiate(prefab_UICard, transform);
        go.GetComponent<S_UICard>().SetCardInfo(card);

        RectTransform rt = go.GetComponent<RectTransform>();
        S_UICard uiCard = go.GetComponent<S_UICard>();

        float effectTime = S_EffectActivator.Instance.GetEffectLifeTime();

        Sequence seq = DOTween.Sequence();

        // 저주
        uiCard.SetCursedEffect(true);

        // Step 0 : 위치와 불투명도 초기화
        rt.anchoredPosition = curseDeckCardStartPos;
        uiCard.SetAlphaValue(0, 0);

        // Step 1: 이동하면서 등장
        // 시각적으로: 카드가 등장 위치에서 목적지로 이동하며 서서히 보이게 됨
        seq.Append(rt.DOAnchorPos(curseDeckCardEndPos, effectTime / 3).SetEase(Ease.OutQuart));
        seq.InsertCallback(0, () => uiCard.SetAlphaValue(1, effectTime / 3));

        // Step 2: 잠시 멈춤
        seq.AppendInterval(effectTime / 3);

        // Step 3: 다시 한 번 이동과 함께 페이드아웃
        seq.Append(rt.DOAnchorPos(curseDeckCardEndPos, effectTime / 3).SetEase(Ease.OutQuart));
        seq.InsertCallback(effectTime / 3 * 2, () => uiCard.SetAlphaValue(0, effectTime / 3));

        // Step 4: 종료 시 오브젝트 파괴
        seq.OnComplete(() => Destroy(go));
    }
    public async Task ExclusionDeckCardVFXAsync(S_Card card)
    {
        GameObject go = Instantiate(prefab_UICard, transform);
        go.GetComponent<S_UICard>().SetCardInfo(card);

        RectTransform rt = go.GetComponent<RectTransform>();
        S_UICard uiCard = go.GetComponent<S_UICard>();

        float effectTime = S_EffectActivator.Instance.GetEffectLifeTime();

        Sequence seq = DOTween.Sequence();

        // Step 0 : 위치와 불투명도 초기화
        rt.anchoredPosition = exclusionDeckCardStartPos;
        uiCard.SetAlphaValue(0, 0);

        // Step 1 : 선명해지면서 이동
        seq.Append(rt.DOAnchorPos(exclusionDeckCardEndPos, effectTime).SetEase(Ease.OutQuart));
        seq.InsertCallback(0, () => uiCard.SetAlphaValue(1, effectTime / 3));

        // Step 2 : 투명해짐
        seq.InsertCallback(effectTime / 3 * 2, () => uiCard.SetAlphaValue(0, effectTime / 3));

        // Step 3 : 종료 시 오브젝트 파괴
        seq.OnComplete(() => Destroy(go));

        await seq.AsyncWaitForCompletion();
    }
    public async Task ReturnExclusionCardsByTwistAsync(List<S_Card> cards)
    {
        foreach (S_Card card in cards)
        {
            GameObject go = Instantiate(prefab_UICard, transform);
            go.GetComponent<S_UICard>().SetCardInfo(card);

            RectTransform rt = go.GetComponent<RectTransform>();
            S_UICard uiCard = go.GetComponent<S_UICard>();

            float effectTime = S_EffectActivator.Instance.GetEffectLifeTime() / 2f;

            Sequence seq = DOTween.Sequence();

            // Step 0 : 위치와 불투명도 초기화
            rt.anchoredPosition = exclusionDeckCardEndPos;
            uiCard.SetAlphaValue(0, 0);

            // Step 1 : 선명해지면서 이동
            seq.Append(rt.DOAnchorPos(exclusionDeckCardStartPos, effectTime).SetEase(Ease.OutQuart));
            seq.InsertCallback(0, () => uiCard.SetAlphaValue(1, effectTime / 3));

            // Step 2 : 종료 시 오브젝트 파괴
            seq.OnComplete(() => Destroy(go));

            await seq.AsyncWaitForCompletion();
        }
    }
    #endregion
    #region 전개 관련
    public void StartExpansionCards()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.HittingCard;

        List<S_Card> cards = S_PlayerCard.Instance.DrawRandomCard(3);

        // 카드 세팅
        foreach (S_Card card in cards)
        {
            GameObject go = Instantiate(prefab_ExpansionUICard, layoutGroup_ExpansionCards.transform);
            go.GetComponent<S_ExpansionUICard>().SetCardInfo(card);

            go.GetComponent<S_ExpansionUICard>().SetAlphaValue(1, SHOW_EXPANSION_TIME);
        }

        // 블락 패널 켜기
        AppearBlockingPanel();

        // 버튼 켜기
        image_HideExpansionCardsBtn.SetActive(true);
        text_HideExpansionCardsBtn.gameObject.SetActive(true);
        image_HideExpansionCardsBtn.GetComponent<Image>().DOKill();
        text_HideExpansionCardsBtn.DOKill();
        image_HideExpansionCardsBtn.GetComponent<Image>().DOFade(1f, SHOW_EXPANSION_TIME);
        text_HideExpansionCardsBtn.DOFade(1f, SHOW_EXPANSION_TIME);

        isShowExpansionCards = true;

        // 히트 버튼 퇴장
        S_HitBtnSystem.Instance.DisappearHitBtn();
    }
    public void EndExpansionCards()
    {
        // 카드 숨기기
        foreach (Transform t in layoutGroup_ExpansionCards.transform)
        {
            t.gameObject.GetComponent<S_ExpansionUICard>().SetAlphaValue(0, SHOW_EXPANSION_TIME);
        }

        // 블락 패널 끄기
        DisappearBlockingPanel();

        // 버튼 끄기 & 전개 카드 파괴
        image_HideExpansionCardsBtn.GetComponent<Image>().DOKill();
        text_HideExpansionCardsBtn.DOKill();
        image_HideExpansionCardsBtn.GetComponent<Image>().DOFade(0f, SHOW_EXPANSION_TIME);
        text_HideExpansionCardsBtn.DOFade(0f, SHOW_EXPANSION_TIME)
            .OnComplete(() =>
            {
                image_HideExpansionCardsBtn.SetActive(false);
                text_HideExpansionCardsBtn.gameObject.SetActive(false);
                DestroyExpansionCards();
            });

        // 히트 버튼 다시 등장
        S_HitBtnSystem.Instance.AppearHitBtn();
    }
    void DestroyExpansionCards()
    {
        foreach (Transform t in layoutGroup_ExpansionCards.transform)
        {
            Destroy(t.gameObject);
        }
    }
    #endregion
    #region 버튼 함수
    public void ClickHideExpansionCardsBtn()
    {
        if (isShowExpansionCards)
        {
            isShowExpansionCards = false;

            // 카드 숨기기
            foreach (Transform t in layoutGroup_ExpansionCards.transform)
            {
                t.gameObject.GetComponent<S_ExpansionUICard>().SetAlphaValue(0, SHOW_EXPANSION_TIME);
            }

            // 블락 패널 끄기
            DisappearBlockingPanel();

            text_HideExpansionCardsBtn.text = "카드 보기";
        }
        else
        {
            isShowExpansionCards = true;

            // 카드 보이기
            foreach (Transform t in layoutGroup_ExpansionCards.transform)
            {
                t.gameObject.GetComponent<S_ExpansionUICard>().SetAlphaValue(1f, SHOW_EXPANSION_TIME);
            }

            // 블락 패널 켜기
            AppearBlockingPanel();

            text_HideExpansionCardsBtn.text = "숨기기";
        }
    }
    #endregion
}
