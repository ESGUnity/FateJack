using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerStat : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerCard pCard;
    S_PlayerSkill pSkill;

    [Header("피조물 관련")]
    [HideInInspector] public int StackSum { get; set; }
    [HideInInspector] public int CurrentLimit { get; set; }
    public const int ORIGIN_LIMIT = 21;

    [Header("시작 능력치 값")]
    const int START_MAX_HEALTH = 3;
    const int START_MAX_DETERMINATION = 3;
    const int START_GOLD = 3;

    [Header("체력")] // 체력의 standDamagedHealth는 시련 시련 내에 절대 변하지 않는다.(비틀기 영향 X) 그래서 히스토리아에 저장하지 않습니다.
    [HideInInspector] public int MaxHealth { get; private set; }
    int currentHealth;
    int standDamagedHealth;
    int additionalHealth; 

    [Header("의지")] // 의지의 useDetermination은 시련 내에 절대 변하지 않는다.(비틀기 영향 X) 그래서 히스토리에 저장하지 않습니디.
    [HideInInspector] public int MaxDetermination { get; private set; }
    int currentDetermination;
    int useDetermination;
    int additionalDetermination;

    [Header("골드")]
    [HideInInspector] public int CurrentGold { get; private set; }

    [Header("전투 능력치")]
    [HideInInspector] public int CurrentStrength { get; private set; }
    [HideInInspector] public int CurrentMind { get; private set; }
    [HideInInspector] public int CurrentLuck { get; private set; }

    [Header("특수 상태")]
    [HideInInspector] public bool IsBurst { get; set; }
    [HideInInspector] public bool IsCleanHit { get; set; }
    [HideInInspector] public bool IsDelusion { get; set; }
    [HideInInspector] public S_FirstEffectEnum IsFirst { get; set; }
    [HideInInspector] public bool IsExpansion { get; set; }

    [Header("히스토리")]
    [HideInInspector] public int h_HitCardCount { get; private set; } // 이번 게임에서 히트한 카드 개수만큼 활성화
    [HideInInspector] public int h_SpadeHitCardCount { get; private set; } // 이번 게임에서 히트한 스페이드 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_HeartHitCardCount { get; private set; } // 이번 게임에서 히트한 하트 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_DiamondHitCardCount { get; private set; } // 이번 게임에서 히트한 다이아몬드 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_CloverHitCardCount { get; private set; } // 이번 게임에서 히트한 클로버 문양 카드 개수만큼 활성화
    [HideInInspector] public int h_HitCardSum { get; private set; } // 이번 게임에서 히트한 카드 숫자만큼 활성화
    [HideInInspector] public int h_SpadeHitCardSum { get; private set; } // 이번 게임에서 히트한 스페이드 문양 카드의 숫자만큼 활성화
    [HideInInspector] public int h_HeartHitCardSum { get; private set; } // 이번 게임에서 히트한 하트 문양 카드의 숫자만큼 활성화
    [HideInInspector] public int h_DiamondHitCardSum { get; private set; } // 이번 게임에서 히트한 다이아몬드 문양 카드의 숫자만큼 활성화
    [HideInInspector] public int h_CloverHitCardSum { get; private set; } // 이번 게임에서 히트한 클로버 문양 카드의 숫자만큼 활성화
    [HideInInspector] public int h_DisengageCount { get; private set; } // 이번 게임에서 제외된 카드 개수만큼 활성화
    Stack<S_StatHistory> statHistoryStack = new();

    // 싱글턴
    static S_PlayerStat instance;
    public static S_PlayerStat Instance { get { return instance; } }

    void Awake()
    {
        // 컴포넌트 할당
        pCard = GetComponent<S_PlayerCard>();
        pSkill = GetComponent<S_PlayerSkill>();

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
        StackSum = 0;

        MaxHealth = START_MAX_HEALTH;
        standDamagedHealth = 0;
        additionalHealth = 0;
        currentHealth = MaxHealth - standDamagedHealth + additionalHealth;

        MaxDetermination = START_MAX_DETERMINATION;
        useDetermination = 0;
        additionalDetermination = 0;
        currentDetermination = MaxDetermination - useDetermination + additionalDetermination;

        CurrentGold = START_GOLD;

        CurrentStrength = 0;
        CurrentMind = 0;
        CurrentLuck = 0;

        IsFirst = S_FirstEffectEnum.None;
        IsDelusion = false;
        IsExpansion = false;
    }
    public void CalcHistory(S_CardOrderTypeEnum type, S_Card hitCard) // 히트 또는 제외 시 히스토리 계산. 카드 낼 때(카드오더큐) 바로 적용
    {
        if ((type == S_CardOrderTypeEnum.BasicHit || type == S_CardOrderTypeEnum.IllusionHit) && hitCard != null)
        {
            // 문양에 따른 히스토리 추가
            h_HitCardCount++;
            h_HitCardSum += hitCard.Number;
            switch (hitCard.Suit)
            {
                case S_CardSuitEnum.Spade:
                    h_SpadeHitCardCount++;
                    h_SpadeHitCardSum += hitCard.Number;
                    break;
                case S_CardSuitEnum.Heart:
                    h_HeartHitCardCount++;
                    h_HeartHitCardSum += hitCard.Number;
                    break;
                case S_CardSuitEnum.Diamond:
                    h_DiamondHitCardCount++;
                    h_DiamondHitCardSum += hitCard.Number;
                    break;
                case S_CardSuitEnum.Clover:
                    h_CloverHitCardCount++;
                    h_CloverHitCardSum += hitCard.Number;
                    break;
            }
        }
        else if (type == S_CardOrderTypeEnum.Exclusion)
        {
            h_DisengageCount++;
        }
    }
    public void SaveStatHistory(S_Card hitCard, S_StatHistoryTriggerEnum trigger) // 히스토리 저장
    {
        S_StatHistory newHistory = new S_StatHistory
        {
            HistoryTrigger = trigger, // 이 히스토리를 발생시킨 곳
            TriggerCard = hitCard, // 트리거 카드

            CurrentLimit = CurrentLimit, // 피조물 관련

            AdditionalHealth = additionalHealth, // 현재 능력치
            AdditionalDetermination = additionalDetermination,
            CurrentGold = CurrentGold,     

            CurrentStrength = CurrentStrength, // 추가 능력치
            CurrentMind = CurrentMind,
            CurrentLuck = CurrentLuck,

            IsFirst = IsFirst, // 특수 상태
            IsDelusion = IsDelusion,
            IsExpansion = IsExpansion,

            H_HitCardCount = h_HitCardCount, // 히스토리
            H_SpadeHitCardCount = h_SpadeHitCardCount,
            H_HeartHitCardCount = h_HeartHitCardCount,
            H_DiamondHitCardCount = h_DiamondHitCardCount,
            H_CloverHitCardCount = h_CloverHitCardCount,

            H_HitCardSum = h_HitCardSum,
            H_SpadeHitCardSum = h_SpadeHitCardSum,
            H_HeartHitCardSum = h_HeartHitCardSum,
            H_DiamondHitCardSum = h_DiamondHitCardSum,
            H_CloverHitCardSum = h_CloverHitCardSum,

            H_DisengageCount = h_DisengageCount
        };

        statHistoryStack.Push(newHistory);
    }
    public void ResetStatsByTwist() // 비틀기 시 호출(의지가 1개 이상인 경우를 항상 가정)
    {
        // 카드와 전리품에 의한 히스토리는 모두 팝하기
        while (statHistoryStack.Peek().HistoryTrigger == S_StatHistoryTriggerEnum.Card || statHistoryStack.Peek().HistoryTrigger == S_StatHistoryTriggerEnum.Skill || statHistoryStack.Peek().HistoryTrigger == S_StatHistoryTriggerEnum.Foe)
        {
            statHistoryStack.Pop();
        }

        // 비틀기로 불러올 히스토리 설정
        S_StatHistory h = statHistoryStack.Peek();

        // 히스토리 되돌리기
        h_HitCardCount = h.H_HitCardCount;
        h_SpadeHitCardCount = h.H_SpadeHitCardCount;
        h_HeartHitCardCount = h.H_HeartHitCardCount;
        h_DiamondHitCardCount = h.H_DiamondHitCardCount;
        h_CloverHitCardCount = h.H_CloverHitCardCount;
        h_HitCardSum = h.H_HitCardSum;
        h_SpadeHitCardSum = h.H_SpadeHitCardSum;
        h_HeartHitCardSum = h.H_HeartHitCardSum;
        h_DiamondHitCardSum = h.H_DiamondHitCardSum;
        h_CloverHitCardSum = h.H_CloverHitCardSum;
        h_DisengageCount = h.H_DisengageCount;

        // 피조물 체력 되돌리기
        S_FoeInfoSystem.Instance.ResetHealthByTwist();

        // 한계 및 숫자 합 되돌리기
        CurrentLimit = h.CurrentLimit;
        ResetStackSum();

        // 체력, 의지, 골드 되돌리기
        additionalHealth = h.AdditionalHealth;
        additionalDetermination = h.AdditionalDetermination;
        CurrentGold = h.CurrentGold;

        // 추가 능력치 되돌리기
        CurrentStrength = h.CurrentStrength;
        CurrentMind = h.CurrentMind;
        CurrentLuck = h.CurrentLuck;

        // 특수 상태 되돌리기
        IsFirst = h.IsFirst;
        IsDelusion = h.IsDelusion;
        IsExpansion = h.IsExpansion;

        CheckBurstAndCleanHit();
        S_StatInfoSystem.Instance.ChangeSpecialAbility();

        // 각종 값 민맥스 체크
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void SetStatsByStand() // 스탠드 시 호출
    {
        // 숫자 합 되돌리기
        ResetStackSum();
    }
    public void ResetStatsByEndTrial() // 피조물 전투 종료 시 호출
    {
        CurrentLimit = ORIGIN_LIMIT;
        ResetStackSum();

        standDamagedHealth = 0;
        additionalHealth = 0;
        currentHealth = MaxHealth - standDamagedHealth + additionalHealth;

        useDetermination = 0;
        additionalDetermination = 0;
        currentDetermination = MaxDetermination - useDetermination + additionalDetermination;

        CurrentStrength = 0;
        CurrentMind = 0;
        CurrentLuck = 0;

        IsFirst = S_FirstEffectEnum.None;
        IsDelusion = false;
        IsExpansion = false;

        CheckBurstAndCleanHit();
        S_StatInfoSystem.Instance.ChangeSpecialAbility();

        // 각종 값 민맥스 체크
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    #endregion
    #region 각종 보조 함수
    public void ResetStackSum()
    {
        StackSum = 0;
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public async Task GetDamagedByStand(int value) // 데미지
    {
        standDamagedHealth += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();

        // 로그 생성
        S_EffectActivator.Instance.GenerateEffectLog($"체력 -{value}");

        // 플레이어 이미지 VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Health);
    }
    public void CheckStatsMinMaxValue() // 각종 스탯 MinMax 제한
    {
        // 체력
        currentHealth = MaxHealth - standDamagedHealth + additionalHealth;
        if (currentHealth <= 0) // 게임 오버
        {
            S_GameOverSystem.Instance.AppearGameOverPanel();
        }
        if (currentHealth > MaxHealth) // 과다 치유는 없다.
        {
            currentHealth = MaxHealth;
            additionalHealth = standDamagedHealth;
        }

        // 의지
        currentDetermination = MaxDetermination - useDetermination + additionalDetermination;
        if (currentDetermination < 0)
        {
            currentDetermination = 0;
            additionalDetermination = useDetermination - MaxDetermination;
        }
        if (currentDetermination > MaxDetermination)
        {
            currentDetermination = MaxDetermination;
            additionalDetermination = useDetermination;
        }

        // 나머지 능력치
        //if (StackSum < 0) StackSum = 0;
        if (CurrentLimit < 0) CurrentLimit = 0;
        if (CurrentGold < 0) CurrentGold = 0;
        if (CurrentStrength < 0) CurrentStrength = 0;
        if (CurrentMind < 0) CurrentMind = 0;
        if (CurrentLuck < 0) CurrentLuck = 0;
    }
    public bool CanUseDetermination() // 의지 사용 가능 여부
    {
        CheckStatsMinMaxValue();
        return currentDetermination > 0;
    }
    public void UseDetermination() // 의지 사용
    {
        useDetermination++;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void CheckBurstAndCleanHit()
    {
        if (StackSum > CurrentLimit) // 스택 합이 한계를 초과했다면
        {
            IsBurst = true;
            IsCleanHit = false;
        }
        else if (StackSum == CurrentLimit) // 스택 합이 한계와 같다면
        {
            IsBurst = false;
            IsCleanHit = true;
        }
        else
        {
            IsBurst = false;
            IsCleanHit = false;
        }
    }
    public int GetCurrentHealth()
    {
        CheckStatsMinMaxValue();
        return currentHealth;
    }
    public int GetCurrentDetermination()
    {
        CheckStatsMinMaxValue();
        return currentDetermination;
    }
    #endregion
    #region 능력치 계산
    public void AddOrSubtractStackSum(int value)
    {
        StackSum += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubtractLimit(int value)
    {
        CurrentLimit += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddStrength(int value)
    {
        CurrentStrength += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddMind(int value)
    {
        CurrentMind += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddLuck(int value)
    {
        CurrentLuck += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubtractHealth(int value)
    {
        additionalHealth += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubtractDetermination(int value)
    {
        additionalDetermination += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void AddOrSubtractGold(int value)
    {
        CurrentGold += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    #endregion
}


public struct S_StatHistory // 비틀기를 위한 각종 능력치 및 특수 상태 저장하는 히스토리 구조체
{
    // 이 효과들을 발생시킨 카드
    public S_StatHistoryTriggerEnum HistoryTrigger;
    public S_Card TriggerCard;

    // 피조물 관련
    public int CurrentLimit;

    // 현재 능력치
    public int AdditionalHealth;
    public int AdditionalDetermination;
    public int CurrentGold;

    // 추가 능력치
    public int CurrentStrength;
    public int CurrentMind;
    public int CurrentLuck;

    // 특수 상태
    public S_FirstEffectEnum IsFirst;
    public bool IsDelusion;
    public bool IsExpansion;

    // 역사
    public int H_HitCardCount;
    public int H_SpadeHitCardCount;
    public int H_HeartHitCardCount;
    public int H_DiamondHitCardCount;
    public int H_CloverHitCardCount;

    public int H_HitCardSum;
    public int H_SpadeHitCardSum;
    public int H_HeartHitCardSum;
    public int H_DiamondHitCardSum;
    public int H_CloverHitCardSum;

    public int H_DisengageCount;
}
public enum S_StatHistoryTriggerEnum
{
    Card,
    Skill,
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