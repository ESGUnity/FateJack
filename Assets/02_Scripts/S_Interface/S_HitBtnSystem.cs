using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_HitBtnSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_ExpansionCardObj;

    [Header("씬 오브젝트")]
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByExpansion;
    [SerializeField] GameObject pos_ExpansionBase;

    [Header("컴포넌트")]
    GameObject panel_HitBtnBase;
    GameObject image_HitBtnBase;
    GameObject image_HoverHitBtnInfoBase;
    TMP_Text text_CleanHitProb;
    TMP_Text text_BurstProb;
    GameObject image_HideExpansionCardsBtn;
    TMP_Text text_HideExpansionCards;

    [Header("전개")]
    List<GameObject> expansionCardObjs = new();
    Vector3 EXPANSION_START_POS = new Vector3(-6f, 0, 0);
    Vector3 EXPANSION_END_POS = new Vector3(6f, 0, 0);
    const float STACK_Z_VALUE = -0.02f;
    const float EXPANSION_TIME = 0.1f;
    Vector3 STACK_CARD_ORIGIN_SCALE = new Vector3(1.7f, 1.7f, 1.7f);

    [Header("UI")]
    Vector2 BTNS_HIDE_POS = new Vector2(0, -140);
    Vector2 BTNS_ORIGIN_POS = new Vector2(0, 85);

    // 싱글턴
    static S_HitBtnSystem instance;
    public static S_HitBtnSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // 히트 버튼 관련 컴포넌트 할당
        panel_HitBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_HitBtnBase")).gameObject;
        image_HitBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HitBtnBase")).gameObject;

        image_HoverHitBtnInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HoverHitBtnInfoBase")).gameObject;
        text_CleanHitProb = Array.Find(texts, c => c.gameObject.name.Equals("Text_CleanHitProb"));
        text_BurstProb = Array.Find(texts, c => c.gameObject.name.Equals("Text_BurstProb"));

        image_HideExpansionCardsBtn = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HideExpansionCardsBtn")).gameObject;
        text_HideExpansionCards = Array.Find(texts, c => c.gameObject.name.Equals("Text_HideExpansionCards"));

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
    S_GameFlowStateEnum prevState;
    void Update()
    {
        // 버스트 및 클린히트를 알려주는 패널. Hit 일 때만 켜지고 아니면 꺼짐. 또한 Hit 켜질 때 마우스 올려져있으면 켜짐
        S_GameFlowStateEnum currentState = S_GameFlowManager.Instance.GameFlowState;
        if (prevState != currentState)
        {
            // 1. 상태가 Hit가 아니면 패널 끔
            if (currentState != S_GameFlowStateEnum.Hit)
            {
                PointerExitOnHitBtn();
            }
            // 2. 이전 상태는 Hit가 아니었고, 현재 상태가 Hit로 바뀐 경우
            else if (prevState != S_GameFlowStateEnum.Hit && currentState == S_GameFlowStateEnum.Hit)
            {
                if (IsPointerOverUIObject(image_HitBtnBase))
                {
                     PointerEnterOnHitBtn();
                }
            }

            prevState = currentState;
        }
    }

    public void AppearHitBtn() // 패널 등장
    {
        panel_HitBtnBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_HitBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_HitBtnBase.GetComponent<RectTransform>().DOAnchorPos(BTNS_ORIGIN_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearHitBtn() // 패널 퇴장
    {
        panel_HitBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_HitBtnBase.GetComponent<RectTransform>().DOAnchorPos(BTNS_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_HitBtnBase.SetActive(false));
    }

    #region 버튼 함수
    public void ClickHitBtnAsync() // 히트, 다이얼로그 시에만 작동
    {
        // Dialog 시스템 전용. 다이얼로그에서 GameFlowState를 Hit으로 바꿔줘야한다.
        S_DialogInfoSystem.Instance.ClickNextBtn();

        if ((S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit) && !S_PlayerStat.Instance.IsBurst && !S_PlayerStat.Instance.IsPerfect && (S_PlayerCard.Instance.GetDeckCards().Count > 0 || S_PlayerCard.Instance.GetUsedCards().Count > 0))
        {
            HitCard();
        }
        else if (S_PlayerStat.Instance.IsBurst || S_PlayerStat.Instance.IsPerfect)
        {
            DialogData dialog = S_DialogMetaData.GetMonologs("Hit_BurstOrPerfect");
            S_DialogInfoSystem.Instance.StartMonolog(dialog.Name, dialog.Dialog, 8);
        }
        else if (S_PlayerCard.Instance.GetDeckCards().Count == 0 && S_PlayerCard.Instance.GetUsedCards().Count == 0)
        {
            DialogData dialog = S_DialogMetaData.GetMonologs("Hit_NoCardInDeckAndUsed");
            S_DialogInfoSystem.Instance.StartMonolog(dialog.Name, dialog.Dialog, 8);
        }
        else
        {
            // 히트가 아닐 때 클릭 시
        }
    }
    public void HitCard()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.HittingCard;

        List<S_CardBase> cards = new();
        if (S_PlayerStat.Instance.IsExpansion > 0)
        {
            cards = S_PlayerCard.Instance.DrawRandomCard(4);
        }
        else
        {
            cards = S_PlayerCard.Instance.DrawRandomCard(2);
        }

        // 카드 세팅
        pos_ExpansionBase.SetActive(true);
        foreach (S_CardBase card in cards)
        {
            GameObject go = Instantiate(prefab_ExpansionCardObj, pos_ExpansionBase.transform, true);
            expansionCardObjs.Add(go);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<S_ExpansionCardObj>().SetCardInfo(card);

            go.GetComponent<S_ExpansionCardObj>().SetAlphaValue(0, 0);
            go.GetComponent<S_ExpansionCardObj>().SetAlphaValue(1, EXPANSION_TIME);
        }
        AlignmentExpansionCards();

        // 블락 패널 켜기
        sprite_BlackBackgroundByExpansion.gameObject.SetActive(true);
        sprite_BlackBackgroundByExpansion.DOFade(0.85f, EXPANSION_TIME);

        // 숨기기 버튼 켜기
        image_HideExpansionCardsBtn.SetActive(true);

        // 히트 버튼 퇴장
        DisappearHitBtn();
    }
    public void ClickStandBtn() // 히트, 다이얼로그 시에만 작동
    {
        // Dialog 시스템 전용. 다이얼로그에서 GameFlowState를 Hit으로 바꿔줘야한다.
        S_DialogInfoSystem.Instance.ClickNextBtn();

        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit)
        {
            S_GameFlowManager.Instance.StartStand();
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
    }
    public async Task SelectHitCard(S_CardBase card) // S_ExpansionCard에서 호출하는 메서드
    {
        // 전개 및 우선 해제
        if (S_PlayerStat.Instance.IsExpansion > 0)
        {
            await S_EffectActivator.Instance.AppliedExpansionAsync();
        }
        if (S_PlayerStat.Instance.IsFirst > 0)
        {
            await S_EffectActivator.Instance.AppliedFirstAsync();
        }

        // 카드 내기
        await S_GameFlowManager.Instance.StartHitCardAsync(card);
    }
    void AlignmentExpansionCards() // 전개 카드를 정렬하기
    {
        List<PRS> originCardPRS = SetExpansionCardsPos(expansionCardObjs.Count);

        for (int i = 0; i < expansionCardObjs.Count; i++)
        {
            // PRS 설정
            expansionCardObjs[i].GetComponent<S_ExpansionCardObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            expansionCardObjs[i].GetComponent<S_ExpansionCardObj>().OriginOrder = (i + 1) * 550;
            expansionCardObjs[i].GetComponent<S_ExpansionCardObj>().SetOrder(expansionCardObjs[i].GetComponent<S_ExpansionCardObj>().OriginOrder);

            // 카드의 위치 설정
            expansionCardObjs[i].transform.DOKill();
            expansionCardObjs[i].transform.DOLocalMove(expansionCardObjs[i].GetComponent<S_ExpansionCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            expansionCardObjs[i].transform.DOLocalRotate(expansionCardObjs[i].GetComponent<S_ExpansionCardObj>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            expansionCardObjs[i].transform.DOScale(expansionCardObjs[i].GetComponent<S_ExpansionCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }
    }
    List<PRS> SetExpansionCardsPos(int cardCount) // 카드 위치 설정하는 메서드
    {
        List<PRS> results = new List<PRS>(cardCount);

        for (int i = 0; i < cardCount; i++)
        {
            float t;

            if (cardCount == 1)
            {
                t = 0.5f;
            }
            else if (cardCount == 2)
            {
                t = (i == 0) ? 0.35f : 0.65f;
            }
            else
            {
                t = (cardCount == 1) ? 0.5f : (float)i / (cardCount - 1);
            }

            Vector3 pos = Vector3.Lerp(EXPANSION_START_POS, EXPANSION_END_POS, t);
            pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);

            Vector3 rot = Vector3.zero;
            Vector3 scale = STACK_CARD_ORIGIN_SCALE;

            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void ClickHideExpansionCardsBtn()
    {
        if (sprite_BlackBackgroundByExpansion.gameObject.activeInHierarchy)
        {
            // 카드 숨기기
            foreach (GameObject go in expansionCardObjs)
            {
                go.GetComponent<S_ExpansionCardObj>().SetAlphaValueAsync(0, EXPANSION_TIME).OnComplete(() => go.SetActive(false));
            }

            // 블락 패널 끄기
            sprite_BlackBackgroundByExpansion.DOKill();
            sprite_BlackBackgroundByExpansion.DOFade(0f, EXPANSION_TIME)
                .OnComplete(() => sprite_BlackBackgroundByExpansion.gameObject.SetActive(false));

            text_HideExpansionCards.text = "카드 보기";
        }
        else
        {
            // 카드 숨기기
            foreach (GameObject go in expansionCardObjs)
            {
                go.SetActive(true);
                go.GetComponent<S_ExpansionCardObj>().SetAlphaValue(1, EXPANSION_TIME);
            }

            // 블락 패널 끄기
            sprite_BlackBackgroundByExpansion.gameObject.SetActive(true);
            sprite_BlackBackgroundByExpansion.DOKill();
            sprite_BlackBackgroundByExpansion.DOFade(0.85f, EXPANSION_TIME);

            text_HideExpansionCards.text = "숨기기";
        }
    }
    public void EndExpansion()
    {
        // 카드 제거
        foreach (GameObject go in expansionCardObjs)
        {
            go.GetComponent<S_ExpansionCardObj>().SetAlphaValueAsync(0, EXPANSION_TIME).OnComplete(() => Destroy(go));
        }
        expansionCardObjs.Clear();

        // 블락 패널 끄기
        sprite_BlackBackgroundByExpansion.DOKill();
        sprite_BlackBackgroundByExpansion.DOFade(0f, EXPANSION_TIME)
            .OnComplete(() => sprite_BlackBackgroundByExpansion.gameObject.SetActive(false));

        // 숨기기 버튼 켜기
        image_HideExpansionCardsBtn.SetActive(false);

        // 히트 버튼 퇴장
        AppearHitBtn();
    }
    #endregion
    #region 버스트 및 완벽 알려주는 기능
    public void RenewProbText()
    {
        int limit = S_PlayerStat.Instance.CurrentLimit;
        int stackSum = S_PlayerStat.Instance.CurrentWeight;
        var preDeckCards = S_PlayerCard.Instance.GetDeckCards();

        if (S_PlayerStat.Instance.IsFirst > 0)
        {
            List<S_CardBase> firstCards = S_PlayerCard.Instance.GetValidCardsByFirst();

            if (firstCards.Count > 0)
            {
                CalculateProb(firstCards, stackSum, limit);
                return;
            }
        }

        // None이거나, 우선 카드가 없을 때
        CalculateProb(preDeckCards, stackSum, limit);
    }
    void CalculateProb(List<S_CardBase> cards, int stackSum, int limit)
    {
        int cleanHitCount = 0;
        int burstCount = 0;
        int totalCount = cards.Count;

        foreach (var c in cards)
        {
            int i = stackSum + c.Weight;
            if (i == limit)
            {
                cleanHitCount++;
            }
            else if (i > limit)
            {
                burstCount++;
            }
        }

        float cleanHitProbF = (float)cleanHitCount / totalCount * 100f;
        float burstProbF = (float)burstCount / totalCount * 100f;

        if (totalCount == 0)
        {
            cleanHitProbF = 0;
            burstProbF = 0;
        }
        text_CleanHitProb.text = $"{cleanHitProbF.ToString("F1")}%";
        text_BurstProb.text = $"{burstProbF.ToString("F1")}%";
    }
    public void PointerEnterOnHitBtn()
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit)
        {
            RenewProbText();

            if (!image_HoverHitBtnInfoBase.activeInHierarchy)
            {
                image_HoverHitBtnInfoBase.SetActive(true);
            }
        }
    }
    public void PointerExitOnHitBtn()
    {
        image_HoverHitBtnInfoBase.SetActive(false);
    }
    #endregion
    #region 보조
    bool IsPointerOverUIObject(GameObject target)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject == target || result.gameObject.transform.IsChildOf(target.transform))
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}