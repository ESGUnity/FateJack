using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerStat : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerCard pCard;

    [Header("피조물 관련")]
    [HideInInspector] public int CurrentWeight { get; set; }
    [HideInInspector] public int CurrentLimit { get; set; }
    public const int ORIGIN_LIMIT = 21;

    [Header("시작 능력치 값")]
    const int START_MAX_HEALTH = 10;
    const int START_MAX_DETERMINATION = 3;

    [Header("체력")] // 체력의 standDamagedHealth는 시련 시련 내에 절대 변하지 않는다.(비틀기 영향 X) 그래서 히스토리아에 저장하지 않습니다.
    [HideInInspector] public int MaxHealth { get; private set; }
    int currentHealth;

    [Header("전투 능력치")]
    [HideInInspector] public int CurrentStr { get; private set; }
    [HideInInspector] public int CurrentMind { get; private set; }
    [HideInInspector] public int CurrentLuck { get; private set; }

    [Header("특수 상태")]
    [HideInInspector] public bool IsBurst { get; set; }
    [HideInInspector] public bool IsPerfect { get; set; }
    [HideInInspector] public int IsDelusion { get; set; }
    [HideInInspector] public int IsFirst { get; set; }
    [HideInInspector] public int IsExpansion { get; set; }
    [HideInInspector] public int IsColdBlood { get; set; }

    [Header("히스토리")]
    [HideInInspector] public int h_HitCardCount { get; private set; } // 이번 게임에서 히트한 카드 개수만큼 활성화
    [HideInInspector] public int h_StrCardCount { get; private set; } // 이번 게임에서 히트한 스페이드 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_MindCardCount { get; private set; } // 이번 게임에서 히트한 하트 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_LuckCardCount { get; private set; } // 이번 게임에서 히트한 다이아몬드 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_CommonCardCount { get; private set; } // 이번 게임에서 히트한 클로버 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_HitCardSum { get; private set; } // 이번 게임에서 히트한 카드 숫자만큼 활성화
    [HideInInspector] public int h_StrCardSum { get; private set; } // 이번 게임에서 히트한 스페이드 문양 카드의 숫자만큼 활성화
    [HideInInspector] public int h_MindCardSum { get; private set; } // 이번 게임에서 히트한 하트 문양 카드의 숫자만큼 활성화
    [HideInInspector] public int h_LuckCardSum { get; private set; } // 이번 게임에서 히트한 다이아몬드 문양 카드의 숫자만큼 활성화
    [HideInInspector] public int h_CommonCardSum { get; private set; } // 이번 게임에서 히트한 클로버 문양 카드의 숫자만큼 활성화
    Stack<S_StatHistory> statHistoryStack = new();

    // 싱글턴
    static S_PlayerStat instance;
    public static S_PlayerStat Instance { get { return instance; } }

    void Awake()
    {
        // 컴포넌트 할당
        pCard = GetComponent<S_PlayerCard>();

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

    #region 스탯 초기화 및 설정
    public void InitStatsByStartGame() // 게임 시작 시 능력치 초기화
    {
        CurrentLimit = ORIGIN_LIMIT;
        CurrentWeight = 0;

        MaxHealth = START_MAX_HEALTH;
        currentHealth = MaxHealth;

        CurrentStr = 0;
        CurrentMind = 0;
        CurrentLuck = 0;

        IsFirst = 0;
        IsDelusion = 0;
        IsExpansion = 0;
        IsColdBlood = 0;
    }
    public void UpdateHistory(S_CardBase hitCard) // 히트 또는 제외 시 히스토리 계산. 카드 낼 때(카드오더큐) 바로 적용
    {
        // 문양에 따른 히스토리 추가
        h_HitCardCount++;
        h_HitCardSum += hitCard.Weight;
        switch (hitCard.CardType)
        {
            case S_CardTypeEnum.Str:
                h_StrCardCount++;
                h_StrCardSum += hitCard.Weight;
                break;
            case S_CardTypeEnum.Mind:
                h_MindCardCount++;
                h_MindCardSum += hitCard.Weight;
                break;
            case S_CardTypeEnum.Luck:
                h_LuckCardCount++;
                h_LuckCardSum += hitCard.Weight;
                break;
            case S_CardTypeEnum.Common:
                h_CommonCardCount++;
                h_CommonCardSum += hitCard.Weight;
                break;
        }
    }
    public void SaveStatHistory(S_CardBase hitCard, S_StatHistoryTriggerEnum trigger) // 히스토리 저장
    {
        S_StatHistory newHistory = new S_StatHistory
        {
            HistoryTrigger = trigger, // 이 히스토리를 발생시킨 곳
            TriggerCard = hitCard, // 트리거 카드

            CurrentLimit = CurrentLimit, // 피조물 관련

            CurrentHealth = currentHealth, // 현재 능력치

            CurrentStrength = CurrentStr, // 추가 능력치
            CurrentMind = CurrentMind,
            CurrentLuck = CurrentLuck,

            IsFirst = IsFirst, // 특수 상태
            IsDelusion = IsDelusion,
            IsExpansion = IsExpansion,
            IsColdBlood = IsColdBlood,

            H_HitCardCount = h_HitCardCount, // 히스토리
            H_StrCardCount = h_StrCardCount,
            H_MindCardCount = h_MindCardCount,
            H_LuckCardCount = h_LuckCardCount,
            H_CommonCardCount = h_CommonCardCount,

            H_HitCardSum = h_HitCardSum,
            H_StrCardSum = h_StrCardSum,
            H_MindCardSum = h_MindCardSum,
            H_LuckCardSum = h_LuckCardSum,
            H_CommonCardSum = h_CommonCardSum,
        };

        statHistoryStack.Push(newHistory);
    }
    public void SetStatsByStand() // 스탠드 시 호출
    {
        // 숫자 합 되돌리기
        ResetCurrentWeight();
    }
    public void ResetStatsByEndTrial() // 시련 종료 시
    {
        CurrentLimit = ORIGIN_LIMIT;
        ResetCurrentWeight();

        currentHealth = MaxHealth;

        CurrentStr = 0;
        CurrentMind = 0;
        CurrentLuck = 0;

        IsDelusion = 0;
        IsFirst = 0;
        IsExpansion = 0;
        IsColdBlood = 0;

        // 버스트와 완벽 체크
        CheckBurstAndPerfect();

        // 각종 값 민맥스 체크
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    #endregion
    #region 각종 보조 함수
    public void ResetCurrentWeight()
    {
        CurrentWeight = 0;
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void CheckStatsMinMaxValue() // 각종 스탯 MinMax 제한
    {
        // 체력
        if (currentHealth <= 0) // 게임 오버
        {
            S_GameOverSystem.Instance.AppearGameOverPanel();
        }
        if (currentHealth > MaxHealth) // 과다 치유는 없다.
        {
            currentHealth = MaxHealth;
        }

        if (CurrentWeight < 0) CurrentWeight = 0;
        if (CurrentLimit < 0) CurrentLimit = 0;
        if (CurrentStr < 0) CurrentStr = 0;
        if (CurrentMind < 0) CurrentMind = 0;
        if (CurrentLuck < 0) CurrentLuck = 0;
    }
    public void CheckBurstAndPerfect()
    {
        int diff = CurrentLimit - CurrentWeight;
        if (diff < 0) // 무게가 한계를 초과한 경우
        {
            IsBurst = true;
            IsPerfect = false;
        }
        else if (CurrentWeight == CurrentLimit) // 무게가 한계와 같은 경우
        {
            IsBurst = false;
            IsPerfect = true;
        }
        else // 무게가 한계보다 작은 경우
        {
            IsBurst = false;
            IsPerfect = false;
        }

        // 지속 효과 : 1만큼 차이나도 완벽을 얻을 수 있음
        if (pCard.GetPersistCardsInField(S_PersistEnum.CheckBurstAndPerfect_CanPerfectDiff1).Count > 0 && Mathf.Abs(diff) == 1)
        {
            IsBurst = false;
            IsPerfect = true;
        }

        S_StatInfoSystem.Instance.UpdateSpecialAbility();
    }
    public int GetCurrentHealth()
    {
        CheckStatsMinMaxValue();
        return currentHealth;
    }
    #endregion
    #region 능력치 계산
    public void AddOrSubHealth(int value)
    {
        currentHealth += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubWeight(int value)
    {
        CurrentWeight += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubLimit(int value)
    {
        CurrentLimit += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubStr(int value)
    {
        CurrentStr += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubMind(int value)
    {
        CurrentMind += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubLuck(int value)
    {
        CurrentLuck += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    #endregion
}


public struct S_StatHistory // 비틀기를 위한 각종 능력치 및 특수 상태 저장하는 히스토리 구조체
{
    // 이 효과들을 발생시킨 카드
    public S_StatHistoryTriggerEnum HistoryTrigger;
    public S_CardBase TriggerCard;

    // 피조물 관련
    public int CurrentLimit;

    // 현재 능력치
    public int CurrentHealth;

    // 추가 능력치
    public int CurrentStrength;
    public int CurrentMind;
    public int CurrentLuck;

    // 특수 상태
    public int IsDelusion;
    public int IsFirst;
    public int IsExpansion;
    public int IsColdBlood;

    // 역사
    public int H_HitCardCount;
    public int H_StrCardCount;
    public int H_MindCardCount;
    public int H_LuckCardCount;
    public int H_CommonCardCount;

    public int H_HitCardSum;
    public int H_StrCardSum;
    public int H_MindCardSum;
    public int H_LuckCardSum;
    public int H_CommonCardSum;
}
public enum S_StatHistoryTriggerEnum
{
    Card,
    Trinket,
    Foe,
    Stand,
    Twist,
    StartTrial,
    EndTrial
}
public enum S_FirstEffectEnum // 우선 효과 열거형
{
    None,
    Spade,
    Heart,
    Diamond,
    Clover,
    LeastSuit,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    CleanHitNumber,
}