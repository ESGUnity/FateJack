using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_StackInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_StackCard;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject spadeStackBase;
    [SerializeField] GameObject heartStackBase;
    [SerializeField] GameObject diamondStackBase;
    [SerializeField] GameObject cloverStackBase;
    [SerializeField] GameObject hitOrderStack1Base;
    [SerializeField] GameObject hitOrderStack2Base;

    [Header("컴포넌트")]
    GameObject panel_SortingBtnBase;
    GameObject pos_EffectLogPos;
    GameObject pos_EffectLogPosTEMP;

    [Header("카드 오브젝트 리스트")]
    List<GameObject> spadeCards = new();
    List<GameObject> heartCards = new();
    List<GameObject> diamondCards = new();
    List<GameObject> cloverCards = new();
    List<GameObject> stackCardObjects = new();

    [Header("스택 카드 애니메이션")]
    Vector3 cardSpawnPoint = new Vector3(0, 0, -9f);
    Vector3 effectLogEndPoint = new Vector3(0, 50, 0);
    public const float EFFECT_LOG_LIFE_TIME = 0.5f;
    public const float HIT_EFFECT_VFX_TIME = 0.2f;

    Vector3 suitBaseStartPoint = new Vector3(-3.5f, 0, 0);
    Vector3 suitBaseEndPoint = new Vector3(3.5f, 0, 0);
    Vector3 hitOrderBaseStartPos = new Vector3(-8.5f, 0, 0);
    Vector3 hitOrderBaseEndPos = new Vector3(8.5f, 0, 0);
    public const float STACK_Z_VALUE = -0.02f;
    const float LEAN_VALUE = 4f;

    Vector3 exclusionPoint = new Vector3(0, 1f, 0);
    public const float EXCLUSION_SCALE_AMOUNT = 1.2f;
    public const float EXCLUSION_TIME = 0.4f;
    public const float EXCLUSION_MOVE_Y_AMOUNT = 10f;

    [Header("카드 정렬")]
    Vector2 sortingBtnHidePos = new Vector2(-180, -80);
    Vector2 sortingBtnOriginPos = new Vector2(10, -80);
    bool isSuitBase;

    // 싱글턴
    static S_StackInfoSystem instance;
    public static S_StackInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // 히트 버튼 관련 컴포넌트 할당
        panel_SortingBtnBase = System.Array.Find(transforms, c => c.gameObject.name.Equals("Panel_SortingBtnBase")).gameObject;
        pos_EffectLogPos = System.Array.Find(transforms, c => c.gameObject.name.Equals("Pos_EffectLogPos")).gameObject;
        pos_EffectLogPosTEMP = System.Array.Find(transforms, c => c.gameObject.name.Equals("Pos_EffectLogPosTEMP")).gameObject;

        // 싱글턴
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

        // 두트윈으로 등장 애니메이션 주기
        panel_SortingBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_SortingBtnBase.GetComponent<RectTransform>().DOAnchorPos(sortingBtnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearStackInfoBtn()
    {
        panel_SortingBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_SortingBtnBase.GetComponent<RectTransform>().DOAnchorPos(sortingBtnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_SortingBtnBase.SetActive(false));
    }

    public async Task HitToStackAsync(S_Card hitCard) // 스택에 카드가 추가될 때 VFX(히트, 창조 등 환상 카드 포함)
    {
        // 스택 카드 생성 및 설정
        GameObject go = Instantiate(prefab_StackCard);
        go.transform.position = cardSpawnPoint;
        go.GetComponent<S_StackCard>().SetCardInfo(hitCard);
        stackCardObjects.Add(go);

        // 문양에 따라 베이스 정해주기
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

        // 카드의 환상과 비틀기 가능 카드에 따른 효과 적용
        UpdateStackCardsState();

        await SortStackVFXAsync();
    }
    public async Task SortStackVFXAsync() // 스택을 정렬하는 카드
    {
        if (isSuitBase)
        {
            foreach (GameObject cardObj in stackCardObjects)
            {
                S_Card card = cardObj.GetComponent<S_StackCard>().CardInfo;
                // 문양에 따라 베이스 정해주기
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
                if (count <= 20)
                {
                    cardObj.transform.SetParent(hitOrderStack1Base.transform, true);
                }
                else
                {
                    cardObj.transform.SetParent(hitOrderStack2Base.transform, true);
                }
                count++;
            }

            await AlignmentStackCardByHitOrderBase();
        }
    }
    public void SortStackVFXOnlyOrder() // 스택을 정렬하는 카드
    {
        if (isSuitBase)
        {
            foreach (GameObject cardObj in stackCardObjects)
            {
                S_Card card = cardObj.GetComponent<S_StackCard>().CardInfo;
                // 문양에 따라 베이스 정해주기
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

            AlignmentStackCardBySuitBaseOnlyOrder();
        }
        else
        {
            int count = 1;
            foreach (GameObject cardObj in stackCardObjects)
            {
                if (count <= 20)
                {
                    cardObj.transform.SetParent(hitOrderStack1Base.transform, true);
                }
                else
                {
                    cardObj.transform.SetParent(hitOrderStack2Base.transform, true);
                }
                count++;
            }

            AlignmentStackCardByHitOrderBaseOnlyOrder();
        }
    }
    async Task AlignmentStackCardBySuitBase() // 문양에 따라 스택 카드 정렬
    {
        // 모든 문양 베이스 리스트 생성
        List<List<GameObject>> suitBases = new List<List<GameObject>> { spadeCards, heartCards, diamondCards, cloverCards };

        // 트윈 태스크 변수 초기화
        List<Task> tweenTask = new List<Task>();

        // 카드 정렬
        foreach (List<GameObject> suitCards in suitBases)
        {
            List<PRS> originCardPRS = SetStackCardPosBySuitBase(suitCards.Count);

            suitCards.Sort((a, b) => a.GetComponent<S_StackCard>().CardInfo.Number.CompareTo(b.GetComponent<S_StackCard>().CardInfo.Number));

            for (int i = 0; i < suitCards.Count; i++)
            {
                // 카드 위치 설정
                suitCards[i].GetComponent<S_StackCard>().OriginPRS = originCardPRS[i];

                // 소팅오더 설정
                suitCards[i].GetComponent<S_StackCard>().OriginOrder = (i + 1) * 10;
                suitCards[i].GetComponent<S_StackCard>().SetOrder(suitCards[i].GetComponent<S_StackCard>().OriginOrder);
                Task move = suitCards[i].GetComponent<Transform>().DOLocalMove(suitCards[i].GetComponent<S_StackCard>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
                Task rotate = suitCards[i].GetComponent<Transform>().DOLocalRotate(suitCards[i].GetComponent<S_StackCard>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
                Task scale = suitCards[i].GetComponent<Transform>().DOScale(suitCards[i].GetComponent<S_StackCard>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();

                tweenTask.Add(Task.WhenAll(move, rotate, scale));
            }
        }

        await Task.WhenAll(tweenTask);
    }
    void AlignmentStackCardBySuitBaseOnlyOrder() // 문양에 따라 스택 카드 정렬
    {
        // 모든 문양 베이스 리스트 생성
        List<List<GameObject>> suitBases = new List<List<GameObject>> { spadeCards, heartCards, diamondCards, cloverCards };

        // 카드 정렬
        foreach (List<GameObject> suitCards in suitBases)
        {
            suitCards.Sort((a, b) => a.GetComponent<S_StackCard>().CardInfo.Number.CompareTo(b.GetComponent<S_StackCard>().CardInfo.Number));

            for (int i = 0; i < suitCards.Count; i++)
            {
                // 소팅오더 설정
                suitCards[i].GetComponent<S_StackCard>().OriginOrder = (i + 1) * 10;
                suitCards[i].GetComponent<S_StackCard>().SetOrder(suitCards[i].GetComponent<S_StackCard>().OriginOrder);
            }
        }
    }
    List<PRS> SetStackCardPosBySuitBase(int cardCount) // 카드 위치 설정하는 메서드
    {
        List<PRS> results = new List<PRS>(cardCount);
        float interval = cardCount <= 10 ? 1f / 9f : 1f / (cardCount - 1);

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(suitBaseStartPoint, suitBaseEndPoint, interval * i);
            pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
            Vector3 rot = new Vector3(0, 0, Random.Range(-LEAN_VALUE, LEAN_VALUE));
            Vector3 scale = new Vector3(1.35f, 1.35f, 1.35f);
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    async Task AlignmentStackCardByHitOrderBase() // 스택에 있는 카드를 정렬하기
    {
        List<GameObject> allStackCards = stackCardObjects.ToList();
        List<PRS> originCardPRS = SetStackCardPosByHitOrderBase(allStackCards.Count);
        List<Task> tweenTask = new List<Task>();

        for (int i = 0; i < allStackCards.Count; i++)
        {
            // 카드 위치 설정
            allStackCards[i].GetComponent<S_StackCard>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            allStackCards[i].GetComponent<S_StackCard>().OriginOrder = (i + 1) * 10;
            allStackCards[i].GetComponent<S_StackCard>().SetOrder(allStackCards[i].GetComponent<S_StackCard>().OriginOrder);
            Task move = allStackCards[i].GetComponent<Transform>().DOLocalMove(allStackCards[i].GetComponent<S_StackCard>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task rotate = allStackCards[i].GetComponent<Transform>().DOLocalRotate(allStackCards[i].GetComponent<S_StackCard>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task scale = allStackCards[i].GetComponent<Transform>().DOScale(allStackCards[i].GetComponent<S_StackCard>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();

            tweenTask.Add(Task.WhenAll(move, rotate, scale));

        }

        await Task.WhenAll(tweenTask);
    }
    void AlignmentStackCardByHitOrderBaseOnlyOrder() // 스택에 있는 카드를 정렬하기
    {
        List<GameObject> allStackCards = stackCardObjects.ToList();

        for (int i = 0; i < allStackCards.Count; i++)
        {
            // 소팅오더 설정
            allStackCards[i].GetComponent<S_StackCard>().OriginOrder = (i + 1) * 10;
            allStackCards[i].GetComponent<S_StackCard>().SetOrder(allStackCards[i].GetComponent<S_StackCard>().OriginOrder);
        }
    }
    List<PRS> SetStackCardPosByHitOrderBase(int cardCount) // 카드 위치 설정하는 메서드
    {
        List<PRS> results = new List<PRS>(cardCount);
        const int maxPerLine = 20;
        const float fixedInterval = 1f / 19f;

        int firstLineCount = Mathf.Min(cardCount, maxPerLine);
        int secondLineCount = cardCount - firstLineCount;

        // 첫 줄
        for (int i = 0; i < firstLineCount; i++)
        {
            Vector3 pos = Vector3.Lerp(hitOrderBaseStartPos, hitOrderBaseEndPos, fixedInterval * i);
            pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
            Vector3 rot = new Vector3(0, 0, Random.Range(-LEAN_VALUE, LEAN_VALUE));
            Vector3 scale = new Vector3(1.35f, 1.35f, 1.35f);
            results.Add(new PRS(pos, rot, scale));
        }

        // 두 번째 줄 (두 번째 줄도 20장 이하라면 fixedInterval 유지, 그 이상은 유동)
        if (secondLineCount > 0)
        {
            float interval = secondLineCount <= maxPerLine ? fixedInterval : 1f / (secondLineCount - 1);

            for (int i = 0; i < secondLineCount; i++)
            {
                Vector3 pos = Vector3.Lerp(hitOrderBaseStartPos, hitOrderBaseEndPos, interval * i);
                pos = new Vector3(pos.x, pos.y, (firstLineCount + i) * STACK_Z_VALUE);
                Vector3 rot = new Vector3(0, 0, Random.Range(-LEAN_VALUE, LEAN_VALUE));
                Vector3 scale = new Vector3(1.35f, 1.35f, 1.35f);
                results.Add(new PRS(pos, rot, scale));
            }
        }

        return results;
    }
    public void BouncingStackCard(S_Card card) // 카드 바운스 효과 실행
    {
        foreach (GameObject cardObj in stackCardObjects)
        {
            if (cardObj.GetComponent<S_StackCard>().CardInfo == card)
            {
                cardObj.GetComponent<S_StackCard>().BouncingVFX();
                break;
            }
        }
    }
    #region 각종 효과 함수
    public async Task ExclusionCardsByTwistAsync(List<S_Card> exclusionCards) // 비틀기에 의한 카드 제외
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

                seq.Append(go.transform.DOScale(go.transform.localScale * EXCLUSION_SCALE_AMOUNT, S_EffectActivator.Instance.GetEffectLifeTime()).SetEase(Ease.OutQuart));
                seq.Join(go.transform.DOLocalMoveY(EXCLUSION_MOVE_Y_AMOUNT, S_EffectActivator.Instance.GetEffectLifeTime()).SetEase(Ease.OutQuart));

                animationTasks.Add(seq.AsyncWaitForCompletion());

                delay += S_EffectActivator.Instance.GetEffectLifeTime() / 3f;
            }
        }
        await Task.WhenAll(animationTasks);

        foreach (GameObject go in exclusionObjects) // 스택 리스트에서 제거하는 부분
        {
            stackCardObjects.Remove(go);
            if (spadeCards.Contains(go)) spadeCards.Remove(go);
            if (heartCards.Contains(go)) heartCards.Remove(go);
            if (diamondCards.Contains(go)) diamondCards.Remove(go);
            if (cloverCards.Contains(go)) cloverCards.Remove(go);

            go.transform.DOKill();
            go.GetComponent<SpriteRenderer>().DOKill(); // Sprite 관련 Tween 제거
            Destroy(go);
        }

        // 정렬하기
        await SortStackVFXAsync();

        UpdateStackCardsState();
    }
    public async Task ResetCardsByEndTrialAsync() // 시련 종료 시 카드를 덱으로 다시 가져오는 메서드
    {
        List<Task> animationTasks = new();
        float delay = 0f;

        foreach (GameObject go in stackCardObjects)
        {
            Sequence seq = DOTween.Sequence();

            seq.PrependInterval(delay);

            seq.AppendCallback(() =>
            {
                go.GetComponent<S_StackCard>().SetAlphaValue(0, S_EffectActivator.Instance.GetHitAndSortCardsTime());
            });

            seq.Append(go.transform.DOMove(cardSpawnPoint, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart))
                .OnComplete(() => Destroy(go));

            animationTasks.Add(seq.AsyncWaitForCompletion());

            delay += S_EffectActivator.Instance.GetHitAndSortCardsTime() / 3f;
        }
        await Task.WhenAll(animationTasks);

        stackCardObjects.Clear();
        spadeCards.Clear();
        heartCards.Clear();
        diamondCards.Clear();
        cloverCards.Clear();
    }
    public void UpdateStackCardsState() // 카드의 저주, 이번 턴에 히트했는가 여부, 환상, 조건 충족 중(뒤에 초록 효과나오는거)의 상태를 모두 업데이트 하여 VFX를 OnOff 합니다.
    {
        foreach (GameObject go in stackCardObjects)
        {
            go.GetComponent<S_StackCard>().UpdateCardState();
        }
    }
    #endregion
    #region 버튼 함수
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
    #region 보조 함수
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
    public GameObject GetCardObject(S_Card card)
    {
        foreach (GameObject go in stackCardObjects)
        {
            if (go.GetComponent<S_StackCard>().CardInfo == card)
            {
                return go;
            }
        }

        Debug.Log("스택에 없는 걸 왜 찾으려 하는가");
        return null;
    }
    #endregion
}
