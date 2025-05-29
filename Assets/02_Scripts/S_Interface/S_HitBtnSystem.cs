using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_HitBtnSystem : MonoBehaviour
{
    [Header("�� ������Ʈ")]
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByTwistBtn;

    [Header("������Ʈ")]
    GameObject panel_HitBtnBase;
    GameObject image_HoverHitBtnInfoBase;
    TMP_Text text_CleanHitProb;
    TMP_Text text_BurstProb;

    [Header("UI")]
    Vector2 btnBaseHidePos = new Vector2(0, -140);
    Vector2 btnBaseOriginPos = new Vector2(0, 85);
    int prevDeckCount;
    int prevStackSum;

    // �̱���
    static S_HitBtnSystem instance;
    public static S_HitBtnSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // ��Ʈ ��ư ���� ������Ʈ �Ҵ�
        panel_HitBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_HitBtnBase")).gameObject;
        image_HoverHitBtnInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HoverHitBtnInfoBase")).gameObject;
        text_CleanHitProb = Array.Find(texts, c => c.gameObject.name.Equals("Text_CleanHitProb"));
        text_BurstProb = Array.Find(texts, c => c.gameObject.name.Equals("Text_BurstProb"));

        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitPos();
    }
    void Update()
    {
        if (prevStackSum != S_PlayerStat.Instance.StackSum && prevDeckCount != S_PlayerCard.Instance.GetPreDeckCards().Count)
        {
            RenewProbText();

            prevStackSum = S_PlayerStat.Instance.StackSum;
            prevDeckCount = S_PlayerCard.Instance.GetPreDeckCards().Count;
        }
        else if (prevStackSum != S_PlayerStat.Instance.StackSum)
        {
            RenewProbText();

            prevStackSum = S_PlayerStat.Instance.StackSum;
        }
        else if (prevDeckCount != S_PlayerCard.Instance.GetPreDeckCards().Count)
        {
            RenewProbText();

            prevDeckCount = S_PlayerCard.Instance.GetPreDeckCards().Count;
        }

        if (S_StackInfoSystem.Instance.GetCurrentTurnHitCardObjects().Count == 0 && isHoverTwist)
        {
            PointerExitOnTwistBtn();
        }
    }

    public void InitPos()
    {
        panel_HitBtnBase.GetComponent<RectTransform>().anchoredPosition = btnBaseHidePos;
        panel_HitBtnBase.SetActive(false);
    }
    public void AppearHitBtn() // �г� ����
    {
        panel_HitBtnBase.SetActive(true);

        // ��Ʈ������ ���� �ִϸ��̼� �ֱ�
        panel_HitBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_HitBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnBaseOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearHitBtn() // �г� ����
    {
        panel_HitBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_HitBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnBaseHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_HitBtnBase.SetActive(false));
    }


    // ��ư �Լ�
    public async void ClickHitBtnAsync() // ��Ʈ ��ư Ŭ��. ���� ��Ʈ ��ư�� S_DeckInfoSystem�� ����
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit && !S_PlayerStat.Instance.IsBurst && S_PlayerCard.Instance.GetPreDeckCards().Count > 0 && S_PlayerCard.Instance.GetPreStackCards().Count <= 48)
        {
            if (S_PlayerStat.Instance.IsExpansion)
            {
                S_UICardEffecter.Instance.ShowExpansionCards();
            }
            else
            {
                S_Card hitCard = S_PlayerCard.Instance.DrawRandomCard(1)[0];

                // ī�� ����
                await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(hitCard, S_CardOrderTypeEnum.BasicHit);

                // �켱�� �־��ٸ� ����
                if (S_PlayerStat.Instance.IsFirst != S_FirstEffectEnum.None) await S_EffectActivator.Instance.AppliedFirstAsync();

                // ��Ʈ ī�� ����
                if (S_GameFlowManager.Instance.GetCardOrderQueueCount() <= 1)
                {
                    await S_GameFlowManager.Instance.StartHittingCard();
                }
            }
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
        else if (S_PlayerStat.Instance.IsBurst)
        {
            S_InGameUISystem.Instance.CreateLog("����Ʈ �ÿ� ��Ʈ�� �� �����ϴ�.");
        }
        else if (S_PlayerCard.Instance.GetPreDeckCards().Count <= 0)
        {
            S_InGameUISystem.Instance.CreateLog("���� ī�尡 �����ϴ�!");
        }
        else if (S_PlayerCard.Instance.GetPreStackCards().Count > 48)
        {
            S_InGameUISystem.Instance.CreateLog("�� �̻� ���ÿ� ī�带 �� �� �����ϴ�. �ִ� ��� : 48��");
        }
    }
    public void ClickTwistBtn() // ��Ʋ�� Ŭ��
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit && !S_PlayerStat.Instance.IsBurst && S_PlayerStat.Instance.CanUseDetermination() && S_GameFlowManager.Instance.IsCurrentTurnHitted())
        {
            S_GameFlowManager.Instance.StartTwist();
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
        else if (S_PlayerStat.Instance.IsBurst)
        {
            S_InGameUISystem.Instance.CreateLog("����Ʈ �ÿ� �� �� ����.");
        }
        else if (!S_PlayerStat.Instance.CanUseDetermination())
        {
            S_InGameUISystem.Instance.CreateLog("������ �����ϴٳ�~");
        }
        else if (!S_GameFlowManager.Instance.IsCurrentTurnHitted())
        {
            S_InGameUISystem.Instance.CreateLog("��Ʋ ����� ����.");
        }
    }
    public void ClickStandBtn() // ���ĵ� Ŭ��
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit)
        {
            S_GameFlowManager.Instance.StartStand();
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
    }
    public async void SelectHitCardByExpansion(S_Card card) // S_ExpansionCard���� ȣ���ϴ� �޼���
    {
        // ī�� ����
        await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(card, S_CardOrderTypeEnum.BasicHit);

        // ���� ����
        await S_EffectActivator.Instance.AppliedExpansionAsync();

        // �켱�� �־��ٸ� ����
        if (S_PlayerStat.Instance.IsFirst != S_FirstEffectEnum.None) await S_EffectActivator.Instance.AppliedFirstAsync();

        // ��Ʈ ī�� ����
        if (S_GameFlowManager.Instance.GetCardOrderQueueCount() <= 1)
        {
            await S_GameFlowManager.Instance.StartHittingCard();
        }
    }


    // ��ư�� ���콺 �ø��� ������ ȣ���� ȿ��
    public void RenewProbText()
    {
        int limit = S_PlayerStat.Instance.CurrentLimit;
        int stackSum = S_PlayerStat.Instance.StackSum;

        int cleanHitCount = 0;
        int burstCount = 0;

        foreach (S_Card c in S_PlayerCard.Instance.GetPreDeckCards())
        {
            int i = stackSum + c.Number;

            if (i == limit)
            {
                cleanHitCount++;
            }
            else if (i > limit)
            {
                burstCount++;
            }
        }

        float cleanHitProbF = (float)cleanHitCount / (float)S_PlayerCard.Instance.GetPreDeckCards().Count * 100;
        float burstProbF = (float)burstCount / (float)S_PlayerCard.Instance.GetPreDeckCards().Count * 100;

        text_CleanHitProb.text = $"{cleanHitProbF.ToString("F1")}%";
        text_BurstProb.text = $"{burstProbF.ToString("F1")}%";
    }
    public void PointerEnterOnHitBtn()
    {
        RenewProbText();

        if (!image_HoverHitBtnInfoBase.activeInHierarchy)
        {
            image_HoverHitBtnInfoBase.SetActive(true);
        }
    }
    public void PointerExitOnHitBtn()
    {
        image_HoverHitBtnInfoBase.SetActive(false);
    }

    bool isHoverTwist = false;
    public void PointerEnterOnTwistBtn()
    {
        if (S_StackInfoSystem.Instance.GetCurrentTurnHitCardObjects().Count != 0 && !isHoverTwist)
        {
            isHoverTwist = true;

            int order = 600;
            foreach (GameObject go in S_StackInfoSystem.Instance.GetCurrentTurnHitCardObjects())
            {
                go.GetComponent<S_StackCard>().SetOrder(order);

                order += 10;
            }

            sprite_BlackBackgroundByTwistBtn.DOKill();
            sprite_BlackBackgroundByTwistBtn.DOFade(0.8f, 0.1f);
        }
    }
    public async void PointerExitOnTwistBtn()
    {
        isHoverTwist = false;

        sprite_BlackBackgroundByTwistBtn.DOKill();
        sprite_BlackBackgroundByTwistBtn.DOFade(0f, 0.1f);

        await S_StackInfoSystem.Instance.SortStackVFXAsync();
    }
}