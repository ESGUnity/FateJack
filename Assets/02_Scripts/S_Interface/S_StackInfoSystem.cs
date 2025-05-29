using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_StackInfoSystem : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject stackCard;
    [SerializeField] GameObject effectText;
    [SerializeField] GameObject strengthVFX;
    [SerializeField] GameObject mindVFX;
    [SerializeField] GameObject luckVFX;
    [SerializeField] GameObject prefab_UICard;

    [Header("�� ������Ʈ")]
    [SerializeField] GameObject spadeStackBase;
    [SerializeField] GameObject heartStackBase;
    [SerializeField] GameObject diamondStackBase;
    [SerializeField] GameObject cloverStackBase;
    [SerializeField] GameObject hitOrderStack1Base;
    [SerializeField] GameObject hitOrderStack2Base;

    [Header("������Ʈ")]
    GameObject panel_SortingBtnBase;
    GameObject pos_EffectLogPos;
    GameObject pos_EffectLogPosTEMP;

    [Header("ī�� ������Ʈ ����Ʈ")]
    List<GameObject> spadeCards = new();
    List<GameObject> heartCards = new();
    List<GameObject> diamondCards = new();
    List<GameObject> cloverCards = new();
    List<GameObject> stackCardObjects = new();

    [Header("���� ī�� �ִϸ��̼�")]
    Vector3 cardSpawnPoint = new Vector3(0, -7.5f, 0);
    Vector3 effectLogEndPoint = new Vector3(0, 50, 0);
    public const float EFFECT_LOG_LIFE_TIME = 0.5f;
    public const float HIT_EFFECT_VFX_TIME = 0.2f;

    Vector3 suitBaseStartPoint = new Vector3(-3.85f, 0, 0);
    Vector3 suitBaseEndPoint = new Vector3(3.85f, 0, 0);
    Vector3 hitOrderBaseStartPoint = new Vector3(-8.45f, 0, 0);
    Vector3 hitOrderBaseEndPoint = new Vector3(8.45f, 0, 0);
    public const float STACK_Z_VALUE = -0.02f;

    Vector3 exclusionPoint = new Vector3(0, 1f, 0);
    public const float EXCLUSION_SCALE_AMOUNT = 1.2f;
    public const float EXCLUSION_TIME = 0.4f;

    [Header("ī�� ����")]
    Vector2 sortingBtnHidePos = new Vector2(-180, -80);
    Vector2 sortingBtnOriginPos = new Vector2(10, -80);
    bool isSuitBase;

    // �̱���
    static S_StackInfoSystem instance;
    public static S_StackInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // ��Ʈ ��ư ���� ������Ʈ �Ҵ�
        panel_SortingBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_SortingBtnBase")).gameObject;
        pos_EffectLogPos = Array.Find(transforms, c => c.gameObject.name.Equals("Pos_EffectLogPos")).gameObject;
        pos_EffectLogPosTEMP = Array.Find(transforms, c => c.gameObject.name.Equals("Pos_EffectLogPosTEMP")).gameObject;

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

    public void InitPos()
    {
        panel_SortingBtnBase.GetComponent<RectTransform>().anchoredPosition = sortingBtnHidePos;
        panel_SortingBtnBase.SetActive(false);
    }
    public void AppearStackInfoBtn()
    {
        panel_SortingBtnBase.SetActive(true);

        // ��Ʈ������ ���� �ִϸ��̼� �ֱ�
        panel_SortingBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_SortingBtnBase.GetComponent<RectTransform>().DOAnchorPos(sortingBtnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearStackInfoBtn()
    {
        panel_SortingBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_SortingBtnBase.GetComponent<RectTransform>().DOAnchorPos(sortingBtnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_SortingBtnBase.SetActive(false));
    }


    public async Task HitToStackAsync(S_Card hitCard) // ���ÿ� ī�尡 �߰��� �� VFX(��Ʈ, â�� �� ȯ�� ī�� ����)
    {
        // ���� ī�� ���� �� ����
        GameObject go = Instantiate(stackCard);
        go.transform.position = cardSpawnPoint;
        go.GetComponent<S_StackCard>().SetCardInfo(hitCard);
        stackCardObjects.Add(go);

        // ���翡 ���� ���̽� �����ֱ�
        switch (hitCard.Suit)
        {
            case S_CardSuitEnum.Spade:
                spadeCards.Add(go);
                break;
            case S_CardSuitEnum.Heart:
                heartCards.Add(go);
                break;
            case S_CardSuitEnum.Diamond:
                diamondCards.Add(go);
                break;
            case S_CardSuitEnum.Clover:
                cloverCards.Add(go);
                break;
        }

        // ī���� ȯ��� ��Ʋ�� ���� ī�忡 ���� ȿ�� ����
        UpdateStackCardsState();

        await SortStackVFXAsync();
    }
    public async Task SortStackVFXAsync() // ������ �����ϴ� ī��
    {
        if (isSuitBase)
        {
            foreach (GameObject cardObj in stackCardObjects)
            {
                S_Card card = cardObj.GetComponent<S_StackCard>().CardInfo;
                // ���翡 ���� ���̽� �����ֱ�
                switch (card.Suit)
                {
                    case S_CardSuitEnum.Spade:
                        cardObj.transform.SetParent(spadeStackBase.transform, true);
                        break;
                    case S_CardSuitEnum.Heart:
                        cardObj.transform.SetParent(heartStackBase.transform, true);
                        break;
                    case S_CardSuitEnum.Diamond:
                        cardObj.transform.SetParent(diamondStackBase.transform, true);
                        break;
                    case S_CardSuitEnum.Clover:
                        cardObj.transform.SetParent(cloverStackBase.transform, true);
                        break;
                }
            }

            await AlignmentStackCardBySuitBase();
        }
        else
        {
            int count = 1;
            foreach (GameObject cardObj in stackCardObjects)
            {
                if (count <= 24)
                {
                    cardObj.transform.SetParent(hitOrderStack1Base.transform, true);
                }
                else if (count > 24)
                {
                    cardObj.transform.SetParent(hitOrderStack2Base.transform, true);
                }
                count++;
            }

            await AlignmentStackCardByHitOrderBase();
        }
    }
    async Task AlignmentStackCardBySuitBase() // ���翡 ���� ���� ī�� ����
    {
        // ��� ���� ���̽� ����Ʈ ����
        List<List<GameObject>> suitBases = new List<List<GameObject>> { spadeCards, heartCards, diamondCards, cloverCards };

        // Ʈ�� �½�ũ ���� �ʱ�ȭ
        List<Task> tweenTask = new List<Task>();

        // ī�� ����
        foreach (List<GameObject> suitCards in suitBases)
        {
            List<PRS> originCardPRS = SetStackCardPosBySuitBase(suitCards.Count);

            suitCards.Sort((a, b) => a.GetComponent<S_StackCard>().CardInfo.Number.CompareTo(b.GetComponent<S_StackCard>().CardInfo.Number));

            for (int i = 0; i < suitCards.Count; i++)
            {
                // ī�� ��ġ ����
                suitCards[i].GetComponent<S_StackCard>().OriginPRS = originCardPRS[i];

                // ���ÿ��� ����
                suitCards[i].GetComponent<S_StackCard>().OriginOrder = (i + 1) * 10;
                suitCards[i].GetComponent<S_StackCard>().SetOrder(suitCards[i].GetComponent<S_StackCard>().OriginOrder);
                Task move = suitCards[i].GetComponent<Transform>().DOLocalMove(suitCards[i].GetComponent<S_StackCard>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
                Task scale = suitCards[i].GetComponent<Transform>().DOScale(suitCards[i].GetComponent<S_StackCard>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();

                tweenTask.Add(Task.WhenAll(move, scale));
            }
        }

        await Task.WhenAll(tweenTask);
    }
    List<PRS> SetStackCardPosBySuitBase(int cardCount) // ī�� ��ġ �����ϴ� �޼���
    {
        float[] lerps = new float[cardCount];
        List<PRS> results = new List<PRS>(cardCount);

        if (cardCount < 8)
        {
            float interval = 1f / 6f;
            for (int i = 0; i < cardCount; i++)
            {
                lerps[i] = interval * i;
            }
        }
        else if (cardCount >= 8)
        {
            float interval = 1f / (cardCount - 1);
            for (int i = 0; i < cardCount; i++)
            {
                lerps[i] = interval * i;
            }
        }

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(suitBaseStartPoint, suitBaseEndPoint, lerps[i]);
            pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
            Vector3 rot = Vector3.zero;
            Vector3 scale = new Vector3(1, 1, 1);
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    async Task AlignmentStackCardByHitOrderBase() // ���ÿ� �ִ� ī�带 �����ϱ�
    {
        List<GameObject> allStackCards = stackCardObjects.ToList();
        List<PRS> originCardPRS = SetStackCardPosByHitOrderBase(allStackCards.Count);
        List<Task> tweenTask = new List<Task>();

        for (int i = 0; i < allStackCards.Count; i++)
        {
            // ī�� ��ġ ����
            allStackCards[i].GetComponent<S_StackCard>().OriginPRS = originCardPRS[i];

            // ���ÿ��� ����
            allStackCards[i].GetComponent<S_StackCard>().OriginOrder = (i + 1) * 10;
            allStackCards[i].GetComponent<S_StackCard>().SetOrder(allStackCards[i].GetComponent<S_StackCard>().OriginOrder);
            Task move = allStackCards[i].GetComponent<Transform>().DOLocalMove(allStackCards[i].GetComponent<S_StackCard>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task scale = allStackCards[i].GetComponent<Transform>().DOScale(allStackCards[i].GetComponent<S_StackCard>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();

            tweenTask.Add(Task.WhenAll(move, scale));
        }

        await Task.WhenAll(tweenTask);
    }
    List<PRS> SetStackCardPosByHitOrderBase(int cardCount) // ī�� ��ġ �����ϴ� �޼���
    {
        float[] lerps = new float[cardCount];
        List<PRS> results = new List<PRS>(cardCount);

        if (cardCount <= 24)
        {
            float interval = 1f / 23f;
            for (int i = 0; i < cardCount; i++)
            {
                lerps[i] = interval * i;
            }
            for (int i = 0; i < cardCount; i++)
            {
                Vector3 pos = Vector3.Lerp(hitOrderBaseStartPoint, hitOrderBaseEndPoint, lerps[i]);
                pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
                Vector3 rot = Vector3.zero;
                Vector3 scale = new Vector3(1, 1, 1);
                results.Add(new PRS(pos, rot, scale));
            }
        }
        else if (cardCount > 48)
        {
            float interval = 1f / 23f;
            for (int i = 0; i < 24; i++)
            {
                lerps[i] = interval * i;
            }
            for (int i = 0; i < 24; i++)
            {
                Vector3 pos = Vector3.Lerp(hitOrderBaseStartPoint, hitOrderBaseEndPoint, lerps[i]);
                pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
                Vector3 rot = Vector3.zero;
                Vector3 scale = new Vector3(1, 1, 1);
                results.Add(new PRS(pos, rot, scale));
            }

            interval = 1f / (cardCount - 25);
            for (int i = 24; i < cardCount; i++)
            {
                lerps[i] = interval * (i - 24);
            }
            for (int i = 24; i < cardCount; i++)
            {
                Vector3 pos = Vector3.Lerp(hitOrderBaseStartPoint, hitOrderBaseEndPoint, lerps[i]);
                pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
                Vector3 rot = Vector3.zero;
                Vector3 scale = new Vector3(1, 1, 1);
                results.Add(new PRS(pos, rot, scale));
            }
        }
        else if (cardCount > 24)
        {
            float interval = 1f / 23f;
            for (int i = 0; i < 24; i++)
            {
                lerps[i] = interval * i;
            }
            for (int i = 0; i < 24; i++)
            {
                Vector3 pos = Vector3.Lerp(hitOrderBaseStartPoint, hitOrderBaseEndPoint, lerps[i]);
                pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
                Vector3 rot = Vector3.zero;
                Vector3 scale = new Vector3(1, 1, 1);
                results.Add(new PRS(pos, rot, scale));
            }
            for (int i = 24; i < cardCount; i++)
            {
                lerps[i] = interval * (i - 24);
            }
            for (int i = 24; i < cardCount; i++)
            {
                Vector3 pos = Vector3.Lerp(hitOrderBaseStartPoint, hitOrderBaseEndPoint, lerps[i]);
                pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
                Vector3 rot = Vector3.zero;
                Vector3 scale = new Vector3(1, 1, 1);
                results.Add(new PRS(pos, rot, scale));
            }
        }

        return results;
    }
    public void BouncingStackCards(List<S_Card> cards) // ī�� �ٿ ȿ�� ����
    {
        foreach (GameObject cardObj in stackCardObjects)
        {
            if (cards.Contains(cardObj.GetComponent<S_StackCard>().CardInfo))
            {
                cardObj.GetComponent<S_StackCard>().BouncingVFX();
            }
        }
    }
    #region ���� ȿ�� �Լ�
    public async Task ExclusionCardsByTwistAsync(List<S_Card> exclusionCards) // ��Ʋ�⿡ ���� ī�� ����
    {
        List<GameObject> exclusionObjects = new();

        List<Task> animationTasks = new();
        float delay = 0f;

        foreach (GameObject go in stackCardObjects)
        {
            if (exclusionCards.Contains(go.GetComponent<S_StackCard>().CardInfo))
            {
                Sequence seq = DOTween.Sequence();

                seq.PrependInterval(delay);

                seq.AppendCallback(() =>
                {
                    go.GetComponent<S_StackCard>().SetAlphaValue(0, S_EffectActivator.Instance.GetEffectLifeTime());
                    exclusionObjects.Add(go);
                });

                seq.Append(go.transform.DOScale(go.transform.localScale * EXCLUSION_SCALE_AMOUNT, S_EffectActivator.Instance.GetEffectLifeTime()).SetEase(Ease.OutQuart))
                    .Join(go.transform.DOMove(go.transform.position + exclusionPoint, S_EffectActivator.Instance.GetEffectLifeTime()).SetEase(Ease.OutQuart));
                
                animationTasks.Add(seq.AsyncWaitForCompletion());

                delay += S_EffectActivator.Instance.GetEffectLifeTime() / 3f;
            }
        }
        await Task.WhenAll(animationTasks);

        foreach (GameObject go in exclusionObjects) // ���� ����Ʈ���� �����ϴ� �κ�
        {
            stackCardObjects.Remove(go);
            if (spadeCards.Contains(go)) spadeCards.Remove(go);
            if (heartCards.Contains(go)) heartCards.Remove(go);
            if (diamondCards.Contains(go)) diamondCards.Remove(go);
            if (cloverCards.Contains(go)) cloverCards.Remove(go);

            go.transform.DOKill();
            Destroy(go);
        }

        // �����ϱ�
        await SortStackVFXAsync();

        UpdateStackCardsState();
    }
    public async Task ResetCardsByEndTrial() // �÷� ���� �� ī�带 ������ �ٽ� �������� �޼���
    {
        List<Task> animationTasks = new();
        float delay = 0f;
        foreach (GameObject go in stackCardObjects)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.PrependInterval(delay);

            sequence.Append(go.transform.DOMove(cardSpawnPoint, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart))
                .OnComplete(() => Destroy(go));

            animationTasks.Add(sequence.AsyncWaitForCompletion());

            delay += S_EffectActivator.Instance.GetHitAndSortCardsTime() / 3f;
        }
        await Task.WhenAll(animationTasks);

        stackCardObjects.Clear();
        spadeCards.Clear();
        heartCards.Clear();
        diamondCards.Clear();
        cloverCards.Clear();
    }
    public void UpdateStackCardsState() // ī���� ����, �̹� �Ͽ� ��Ʈ�ߴ°� ����, ȯ��, ���� ���� ��(�ڿ� �ʷ� ȿ�������°�)�� ���¸� ��� ������Ʈ �Ͽ� VFX�� OnOff �մϴ�.
    {
        foreach (GameObject go in stackCardObjects)
        {
            go.GetComponent<S_StackCard>().UpdateCardState();
        }
    }
    #endregion
    #region ��ư �Լ�
    bool canClickBtn = true;
    public async void ClickHitOrderSortingBtn()
    {
        if ((S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.HittingCard) && canClickBtn && isSuitBase)
        {
            canClickBtn = false;
            isSuitBase = false;
            await SortStackVFXAsync();

            canClickBtn = true;
        }
    }
    public async void ClickSuitSortingBtn()
    {
        if ((S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.HittingCard) && canClickBtn && !isSuitBase)
        {
            canClickBtn = false;
            isSuitBase = true;
            await SortStackVFXAsync();

            canClickBtn = true;
        }
    }
    #endregion
    #region ���� �Լ�
    public List<GameObject> GetCurrentTurnHitCardObjects()
    {
        List<GameObject> list = new();

        foreach (GameObject go in stackCardObjects)
        {
            if (go != null && go.GetComponent<S_StackCard>().CardInfo.IsCurrentTurnHit)
            {
                list.Add(go);
            }
        }

        return list;
    }
    #endregion
}
