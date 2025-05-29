using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    // 루트 덱 관련
    List<CardEffect> currentGameCardEffects = new(); // 모든 카드 효과 중 이번 게임에서만 사용될 효과의 리스트
    public List<int> RootNumber = new(); // 루트 숫자 리스트
    float voidProbability;
    float normalProbability;
    float rareProbability;
    float epicProbability;
    float mythicProbability;

    // 최대 숫자 및 카드 종류 한계
    const int MAXNUMBER = 11;
    const int FINALCARDSLIMIT = 72;

    // 연출 효과 관련
    const float TOSS_CARD_TIME = 0.5f; // 카드를 토스해주는 시간
    [SerializeField] GameObject probabilityTable;
    [SerializeField] TMP_Text voidText;
    [SerializeField] TMP_Text normalText;
    [SerializeField] TMP_Text rareText;
    [SerializeField] TMP_Text epicText;
    [SerializeField] TMP_Text mythicText;

    // 싱글턴
    static CardManager instance;
    public static CardManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateRootNumber() // 루트 숫자 생성
    {
        if (IsServer)
        {
            for (int i = 0; i <= 5; i++)
            {
                for (int j = 0; j <= 11; j++)
                {
                    RootNumber.Add(j);
                }
            }
        }
    }
    [ClientRpc]
    public void ActiveProbabilityTableClientRpc() // 카드 효과 확률표 활성화
    {
        probabilityTable.SetActive(true);
    }
    public void SetProbability(int round) // 라운드가 변경될 때마다 호출
    {
        switch (round)
        {
            case 1:
                voidProbability = 0.80f;
                normalProbability = 0.20f;
                rareProbability = 0f;
                epicProbability = 0f;
                mythicProbability = 0f;
                break;
            case 2:
                voidProbability = 0.75f;
                normalProbability = 0.20f;
                rareProbability = 0.05f;
                epicProbability = 0f;
                mythicProbability = 0f;
                break;
            case 4:
                voidProbability = 0.30f;
                normalProbability = 0.50f;
                rareProbability = 0.20f;
                epicProbability = 0f;
                mythicProbability = 0f;
                break;
            case 6:
                voidProbability = 0.15f;
                normalProbability = 0.30f;
                rareProbability = 0.35f;
                epicProbability = 0.15f;
                mythicProbability = 0.05f;
                break;
            case 8:
                voidProbability = 0.05f;
                normalProbability = 0.25f;
                rareProbability = 0.35f;
                epicProbability = 0.25f;
                mythicProbability = 0.10f;
                break;
            case 10:
                voidProbability = 0.03f;
                normalProbability = 0.15f;
                rareProbability = 0.30f;
                epicProbability = 0.35f;
                mythicProbability = 0.17f;
                break;
        }

        SetCountTextClientRpc();
    }
    [ClientRpc]
    void SetCountTextClientRpc() // 루트덱이랑 번파일 글자 업데이트
    {
        voidText.text = $"{Mathf.RoundToInt(voidProbability) * 100}%";
        normalText.text = $"{Mathf.RoundToInt(normalProbability) * 100}%";
        rareText.text = $"{Mathf.RoundToInt(rareProbability) * 100}%";
        epicText.text = $"{Mathf.RoundToInt(epicProbability) * 100}%";
        mythicText.text = $"{Mathf.RoundToInt(mythicProbability) * 100}%";
    }
    Card DrawCard() // 루트 숫자에서 하나를 정하고 확률에 따라 카드 효과를 뽑아서 주는 메서드
    {
        if (IsServer)
        {
            // 루트 숫자에서 숫자 하나 랜덤하게 가져오기
            int randomIndex = Random.Range(0, RootNumber.Count);
            int num = RootNumber[randomIndex];
            RootNumber.RemoveAt(randomIndex);

            // 각 카드의 확률 구간에 따라 카드를 선택
            CardEffect pickCardEffect = null;
            float randomValue = Random.Range(0f, 1f);

            if (randomValue < voidProbability)
            {
                pickCardEffect = CardEffectList.PickCardEffectByRank(CardEffectRankEnum.Void);
            }
            else if (randomValue < voidProbability + normalProbability)
            {
                pickCardEffect = CardEffectList.PickCardEffectByRank(CardEffectRankEnum.Normal);
            }
            else if (randomValue < voidProbability + normalProbability + rareProbability)
            {
                pickCardEffect = CardEffectList.PickCardEffectByRank(CardEffectRankEnum.Rare);
            }
            else if (randomValue < voidProbability + normalProbability + rareProbability + epicProbability)
            {
                pickCardEffect = CardEffectList.PickCardEffectByRank(CardEffectRankEnum.Epic);
            }
            else if (randomValue < voidProbability + normalProbability + rareProbability + epicProbability + mythicProbability)
            {
                pickCardEffect = CardEffectList.PickCardEffectByRank(CardEffectRankEnum.Mythic);
            }

            return new Card(num, pickCardEffect.Key);
        }
        else
        {
            return default;
        }
    }
    [ClientRpc]
    public void TossCardToPlayerClientRpc(ulong targetId) // 플레이어에게 카드 한장을 던져주는 시각 효과와 더불어 카드를 주는 메서드
    {
        GameObject go = TossedCardPool.Instance.GetTossedCard();

        // 카드 연출 효과
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(go.transform.DOMove(CoreGameManager.Instance.AllPlayers[targetId].TossCardPos.Value, TOSS_CARD_TIME).SetEase(Ease.OutQuart))
            .Join(go.transform.DORotate(new Vector3(90, 360 * 4.5f, 0), TOSS_CARD_TIME).SetEase(Ease.OutQuart))
            .InsertCallback(0.2f, () => 
            {
                if (IsServer) CoreGameManager.Instance.AllPlayers[targetId].playerCards.AddPlayerDeckClientRpc(DrawCard());
            })
            .OnComplete(() =>
            {
                TossedCardPool.Instance.ReturnCard(go);
            });
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddNumberToRootNumberServerRpc(int number)
    {
        RootNumber.Add(number);
    }
}
