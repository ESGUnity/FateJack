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
    [SerializeField] GameObject pos_StackBase;

    [Header("컴포넌트")]
    GameObject pos_EffectLogPos;

    [Header("카드 오브젝트 리스트")]
    List<GameObject> stackCardObjs = new();

    [Header("연출")]
    Vector3 cardSpawnPoint = new Vector3(0, 0, -9f);
    Vector3 effectLogEndPoint = new Vector3(0, 50, 0);
    const float GEN_CARD_SPAWN_Z_AMOUNT = -10;

    Vector3 STACK_BASE_START_POS = new Vector3(-8.5f, 0, 0);
    Vector3 STACK_BASE_END_POS = new Vector3(8.5f, 0, 0);
    Vector3 STACK_CARD_ORIGIN_SCALE = new Vector3(1.7f, 1.7f, 1.7f);
    const float STACK_Z_VALUE = -0.02f;
    const float LEAN_VALUE = 4f;
    const float CARD_POS_OFFSET = 3f;
    const int MAX_CARD_COUNT = 15; // 고정 격차로 배치되는 최대 카드 개수

    public const float EXCLUSION_SCALE_AMOUNT = 1.2f;
    public const float EXCLUSION_TIME = 0.4f;
    public const float EXCLUSION_MOVE_Y_AMOUNT = 10f;

    // 싱글턴
    static S_StackInfoSystem instance;
    public static S_StackInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        // 히트 버튼 관련 컴포넌트 할당
        pos_EffectLogPos = System.Array.Find(transforms, c => c.gameObject.name.Equals("Pos_EffectLogPos")).gameObject;

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

    public async Task HitToStackAsync(S_Card hitCard) // 스택에 카드가 추가될 때 VFX(히트, 창조 등 환상 카드 포함)
    {
        // 스택 카드 생성 및 설정
        GameObject go = Instantiate(prefab_StackCard);
        go.transform.position = cardSpawnPoint;
        go.transform.SetParent(pos_StackBase.transform, true);
        go.GetComponent<S_StackCardObj>().SetCardInfo(hitCard);
        stackCardObjs.Add(go);

        await AlignmentStackBase();
    }
    async Task AlignmentStackBase() // 스택에 있는 카드를 정렬하기
    {
        List<GameObject> allStackCards = stackCardObjs.ToList();
        List<PRS> originCardPRS = SetStackCardPos(allStackCards.Count);
        List<Task> tweenTask = new();

        for (int i = 0; i < allStackCards.Count; i++)
        {
            // 카드 위치 설정
            allStackCards[i].GetComponent<S_StackCardObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            allStackCards[i].GetComponent<S_StackCardObj>().OriginOrder = (i + 1) * 10;
            allStackCards[i].GetComponent<S_StackCardObj>().SetOrder(allStackCards[i].GetComponent<S_StackCardObj>().OriginOrder);

            // 두트윈
            Task move = allStackCards[i].transform.DOLocalMove(allStackCards[i].GetComponent<S_StackCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task rotate = allStackCards[i].transform.DOLocalRotate(allStackCards[i].GetComponent<S_StackCardObj>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task scale = allStackCards[i].transform.DOScale(allStackCards[i].GetComponent<S_StackCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            tweenTask.Add(Task.WhenAll(move, rotate, scale));
        }

        await Task.WhenAll(tweenTask);
    }
    List<PRS> SetStackCardPos(int cardCount) // 카드 위치 설정하는 메서드
    {
        List<PRS> results = new List<PRS>(cardCount);
        float interval = cardCount <= MAX_CARD_COUNT ? 1f / 14f : 1f / (cardCount - 1);

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(STACK_BASE_START_POS, STACK_BASE_END_POS, interval * i);
            pos = new Vector3(pos.x + Random.Range(-CARD_POS_OFFSET, CARD_POS_OFFSET), pos.y + Random.Range(-CARD_POS_OFFSET, CARD_POS_OFFSET), i * STACK_Z_VALUE);

            Vector3 rot = new Vector3(0, 0, Random.Range(-LEAN_VALUE, LEAN_VALUE));

            Vector3 scale = STACK_CARD_ORIGIN_SCALE;

            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void BouncingStackCard(S_Card card) // 카드 바운스 효과 실행
    {
        foreach (GameObject cardObj in stackCardObjs)
        {
            if (cardObj.GetComponent<S_StackCardObj>().CardInfo == card)
            {
                cardObj.GetComponent<S_StackCardObj>().BouncingVFX();
                break;
            }
        }
    }
    public async Task ExclusionCardAsync(S_Card card) // Legacy. 미사용.
    {
        // 카드 빼기

        // 정렬
        await AlignmentStackBase();
    }
    #region 각종 효과 함수
    public async Task ReturnCardsByTwistAsync(List<S_Card> returnCards)
    {
        List<GameObject> returnObj = new();

        List<Task> animationTasks = new();
        float delay = 0f;

        foreach (GameObject go in stackCardObjs)
        {
            if (returnCards.Contains(go.GetComponent<S_StackCardObj>().CardInfo))
            {
                Sequence seq = DOTween.Sequence();

                seq.PrependInterval(delay);

                seq.AppendCallback(() =>
                {
                    returnObj.Add(go);
                });

                if (go.GetComponent<S_StackCardObj>().CardInfo.IsGenerated)
                {
                    seq.Append(go.transform.DOLocalMoveZ(GEN_CARD_SPAWN_Z_AMOUNT, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart));
                }
                else
                {
                    seq.Append(go.transform.DOMove(cardSpawnPoint, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart));
                }

                animationTasks.Add(seq.AsyncWaitForCompletion());

                delay += S_EffectActivator.Instance.GetHitAndSortCardsTime() / 3f;
            }
        }
        await Task.WhenAll(animationTasks);

        foreach (GameObject go in returnObj) // 스택 오브젝트 리스트에서 제거 및 디스트로이까지
        {
            stackCardObjs.Remove(go);
            Destroy(go);
        }

        // 정렬하기
        await AlignmentStackBase();
    }
    public async Task ResetCardsByEndTrialAsync() // 시련 종료 시 카드를 덱으로 다시 가져오는 메서드
    {
        List<Task> animationTasks = new();
        float delay = 0f;

        foreach (GameObject go in stackCardObjs)
        {
            Sequence seq = DOTween.Sequence();

            seq.PrependInterval(delay);

            // 생성된 카드는 날라가고 내 카드만 덱으로 돌아오게 연출
            if (go.GetComponent<S_StackCardObj>().CardInfo.IsGenerated)
            {
                seq.Append(go.transform.DOLocalMoveZ(GEN_CARD_SPAWN_Z_AMOUNT, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart)).OnComplete(() => Destroy(go));
            }
            else
            {
                seq.Append(go.transform.DOMove(cardSpawnPoint, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart)).OnComplete(() => Destroy(go));
            }

            animationTasks.Add(seq.AsyncWaitForCompletion());

            delay += S_EffectActivator.Instance.GetHitAndSortCardsTime() / 3f;
        }
        await Task.WhenAll(animationTasks);

        stackCardObjs.Clear();
    }
    public void UpdateStackCardState() // 카드의 저주, 이번 턴에 히트했는가 여부, 환상, 각인 조건 충족의 상태를 모두 업데이트 하여 VFX를 OnOff 합니다.
    {
        foreach (GameObject go in stackCardObjs)
        {
            go.GetComponent<S_StackCardObj>().UpdateCardState();
        }
    }
    #endregion
    #region 보조 함수 && 비틀기 호버링 시 사용하는 것들
    public void AlignmentStackBaseOnlyOrder() // 바뀌었던 Order만 다시 원위치하는 메서드
    {
        List<GameObject> allStackCards = stackCardObjs.ToList();

        for (int i = 0; i < allStackCards.Count; i++)
        {
            // 소팅오더 설정
            allStackCards[i].GetComponent<S_StackCardObj>().OriginOrder = (i + 1) * 10;
            allStackCards[i].GetComponent<S_StackCardObj>().SetOrder(allStackCards[i].GetComponent<S_StackCardObj>().OriginOrder);
        }
    }
    public List<GameObject> GetCurrentTurnCardObjs()
    {
        List<GameObject> list = new();

        foreach (GameObject go in stackCardObjs)
        {
            if (go != null && go.GetComponent<S_StackCardObj>().CardInfo.IsCurrentTurn)
            {
                list.Add(go);
            }
        }

        return list;
    }
    public GameObject GetStackCardObj(S_Card card)
    {
        foreach (GameObject go in stackCardObjs)
        {
            if (go.GetComponent<S_StackCardObj>().CardInfo == card)
            {
                return go;
            }
        }

        Debug.Log("스택에 없는 걸 왜 찾으려 하는가");
        return null;
    }

    public async Task SwapCardObjIndex(S_Card card, bool isMoveRight) // 오브젝트 인덱스 교환하는 메서드
    {
        if (isMoveRight)
        {
            int index = stackCardObjs.IndexOf(GetStackCardObj(card));

            if (index == stackCardObjs.Count - 1) // 마지막 인덱스라면
            {
                GameObject lastCard = stackCardObjs[index];

                // 리스트의 요소들을 뒤에서부터 한 칸씩 뒤로 밀기
                for (int i = index; i > 0; i--)
                {
                    stackCardObjs[i] = stackCardObjs[i - 1];
                }

                // 맨 앞에 마지막 카드를 넣기
                stackCardObjs[0] = lastCard;
            }
            else // 아니라면
            {
                // 오른쪽 요소와 자리 바꾸기
                GameObject tempGo = stackCardObjs[index + 1];
                stackCardObjs[index + 1] = stackCardObjs[index];
                stackCardObjs[index] = tempGo;
            }
        }
        else
        {
            int index = stackCardObjs.IndexOf(GetStackCardObj(card));

            if (index == 0)
            {
                GameObject firstCard = stackCardObjs[0];

                // 요소들을 왼쪽으로 한 칸씩 이동
                for (int i = 0; i < stackCardObjs.Count - 1; i++)
                {
                    stackCardObjs[i] = stackCardObjs[i + 1];
                }

                // 마지막 자리에 처음 요소 넣기
                stackCardObjs[stackCardObjs.Count - 1] = firstCard;
            }
            else
            {
                // 일반적인 왼쪽 교체
                GameObject tempGo = stackCardObjs[index - 1];
                stackCardObjs[index - 1] = stackCardObjs[index];
                stackCardObjs[index] = tempGo;
            }
        }

        // 위치 바꿨으니 정렬
        await AlignmentStackBase();
    }
    #endregion
}
