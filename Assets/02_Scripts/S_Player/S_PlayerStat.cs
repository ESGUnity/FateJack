using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

// NOTIFY : ���� ���ĵ� �� ī�带 ���� ����� ������ ����... �����ܵ�!!!!!!!!!!!!!!!!!
// NOTIFY : �׻� ���ؿ� â���� �����Ѵٸ� �׻� ���ظ� ���� �ְ� â�� �ε� �����ض�!!!!!!!!!!!!

public class S_PlayerStat : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject playerAttackVFX;

    [Header("������Ʈ")]
    S_PlayerCard pCard;
    S_PlayerSkill pSkill;

    [Header("������ ����")]
    [HideInInspector] public int StackSum { get; set; }
    [HideInInspector] public int CurrentLimit { get; set; }
    public const int ORIGIN_LIMIT = 21;

    [Header("���� �ɷ�ġ ��")]
    const int START_MAX_HEALTH = 3;
    const int START_MAX_DETERMINATION = 3;
    const int START_GOLD = 3;

    [Header("ü��")] // ü���� standDamagedHealth�� �÷� �÷� ���� ���� ������ �ʴ´�.(��Ʋ�� ���� X) �׷��� �����丮�ƿ� �������� �ʽ��ϴ�.
    [HideInInspector] public int MaxHealth { get; private set; }
    int currentHealth;
    int standDamagedHealth;
    int additionalHealth; 

    [Header("����")] // ������ useDetermination�� �÷� ���� ���� ������ �ʴ´�.(��Ʋ�� ���� X) �׷��� �����丮�� �������� �ʽ��ϵ�.
    [HideInInspector] public int MaxDetermination { get; private set; }
    int currentDetermination;
    int useDetermination;
    int additionalDetermination;

    [Header("���")]
    [HideInInspector] public int CurrentGold { get; private set; }

    [Header("���� �ɷ�ġ")]
    [HideInInspector] public int CurrentStrength { get; private set; }
    [HideInInspector] public int CurrentMind { get; private set; }
    [HideInInspector] public int CurrentLuck { get; private set; }

    [Header("Ư�� ����")]
    [HideInInspector] public bool IsBurst { get; set; }
    [HideInInspector] public bool IsCleanHit { get; set; }
    [HideInInspector] public bool IsDelusion { get; set; }
    [HideInInspector] public S_FirstEffectEnum IsFirst { get; set; }
    [HideInInspector] public bool IsExpansion { get; set; }

    [Header("�����丮")]
    [HideInInspector] public int h_HitCardCount { get; private set; } // �̹� ���ӿ��� ��Ʈ�� ī�� ������ŭ Ȱ��ȭ
    [HideInInspector] public int h_SpadeHitCardCount { get; private set; } // �̹� ���ӿ��� ��Ʈ�� �����̵� ���� ī�� ������ŭ Ȱ��ȭ
    [HideInInspector] public int h_HeartHitCardCount { get; private set; } // �̹� ���ӿ��� ��Ʈ�� ��Ʈ ���� ī�� ������ŭ Ȱ��ȭ
    [HideInInspector] public int h_DiamondHitCardCount { get; private set; } // �̹� ���ӿ��� ��Ʈ�� ���̾Ƹ�� ���� ī�� ������ŭ Ȱ��ȭ
    [HideInInspector] public int h_CloverHitCardCount { get; private set; } // �̹� ���ӿ��� ��Ʈ�� Ŭ�ι� ���� ī�� ������ŭ Ȱ��ȭ
    [HideInInspector] public int h_HitCardSum { get; private set; } // �̹� ���ӿ��� ��Ʈ�� ī�� ���ڸ�ŭ Ȱ��ȭ
    [HideInInspector] public int h_SpadeHitCardSum { get; private set; } // �̹� ���ӿ��� ��Ʈ�� �����̵� ���� ī���� ���ڸ�ŭ Ȱ��ȭ
    [HideInInspector] public int h_HeartHitCardSum { get; private set; } // �̹� ���ӿ��� ��Ʈ�� ��Ʈ ���� ī���� ���ڸ�ŭ Ȱ��ȭ
    [HideInInspector] public int h_DiamondHitCardSum { get; private set; } // �̹� ���ӿ��� ��Ʈ�� ���̾Ƹ�� ���� ī���� ���ڸ�ŭ Ȱ��ȭ
    [HideInInspector] public int h_CloverHitCardSum { get; private set; } // �̹� ���ӿ��� ��Ʈ�� Ŭ�ι� ���� ī���� ���ڸ�ŭ Ȱ��ȭ
    [HideInInspector] public int h_DisengageCount { get; private set; } // �̹� ���ӿ��� ���ܵ� ī�� ������ŭ Ȱ��ȭ
    Stack<S_StatHistory> statHistoryStack = new();

    // �̱���
    static S_PlayerStat instance;
    public static S_PlayerStat Instance { get { return instance; } }

    void Awake()
    {
        // ������Ʈ �Ҵ�
        pCard = GetComponent<S_PlayerCard>();
        pSkill = GetComponent<S_PlayerSkill>();

        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitStatsByStartGame() // ���� ���� �� �ɷ�ġ �ʱ�ȭ
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
    public void CalcHistory(S_CardOrderTypeEnum type, S_Card hitCard) // ��Ʈ �Ǵ� ���� �� �����丮 ���. ī�� �� ��(ī�����ť) �ٷ� ����
    {
        if ((type == S_CardOrderTypeEnum.BasicHit || type == S_CardOrderTypeEnum.IllusionHit) && hitCard != null)
        {
            // ���翡 ���� �����丮 �߰�
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
    public void SaveStatHistory(S_Card hitCard, S_StatHistoryTriggerEnum trigger) // �����丮 ����
    {
        S_StatHistory newHistory = new S_StatHistory
        {
            HistoryTrigger = trigger, // �� �����丮�� �߻���Ų ��
            TriggerCard = hitCard, // Ʈ���� ī��

            CurrentLimit = CurrentLimit, // ������ ����

            AdditionalHealth = additionalHealth, // ���� �ɷ�ġ
            AdditionalDetermination = additionalDetermination,
            CurrentGold = CurrentGold,     

            CurrentStrength = CurrentStrength, // �߰� �ɷ�ġ
            CurrentMind = CurrentMind,
            CurrentLuck = CurrentLuck,

            IsFirst = IsFirst, // Ư�� ����
            IsDelusion = IsDelusion,
            IsExpansion = IsExpansion,

            H_HitCardCount = h_HitCardCount, // �����丮
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

    // ���� ���� ��, ��Ʋ��, ���ĵ�, ������ ���� ����
    public void ResetStatsByTwist() // ��Ʋ�� �� ȣ��(������ 1�� �̻��� ��츦 �׻� ����)
    {
        // ī��� ����ǰ�� ���� �����丮�� ��� ���ϱ�
        while (statHistoryStack.Peek().HistoryTrigger == S_StatHistoryTriggerEnum.Card || statHistoryStack.Peek().HistoryTrigger == S_StatHistoryTriggerEnum.Skill || statHistoryStack.Peek().HistoryTrigger == S_StatHistoryTriggerEnum.Foe)
        {
            statHistoryStack.Pop();
        }

        // ��Ʋ��� �ҷ��� �����丮 ����
        S_StatHistory h = statHistoryStack.Peek();

        // �����丮 �ǵ�����
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

        // ������ ü�� �ǵ�����
        S_FoeInfoSystem.Instance.ResetHealthByTwist();

        // �Ѱ� �� ���� �� �ǵ�����
        CurrentLimit = h.CurrentLimit;
        ResetStackSum();

        // ü��, ����, ��� �ǵ�����
        additionalHealth = h.AdditionalHealth;
        additionalDetermination = h.AdditionalDetermination;
        CurrentGold = h.CurrentGold;

        // �߰� �ɷ�ġ �ǵ�����
        CurrentStrength = h.CurrentStrength;
        CurrentMind = h.CurrentMind;
        CurrentLuck = h.CurrentLuck;

        // Ư�� ���� �ǵ�����
        IsFirst = h.IsFirst;
        IsDelusion = h.IsDelusion;
        IsExpansion = h.IsExpansion;

        CheckBurstAndCleanHit();
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();

        // ���� �� �θƽ� üũ
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public void SetStatsByStand() // ���ĵ� �� ȣ��
    {
        // ���� �� �ǵ�����
        ResetStackSum();
    }
    public void ResetStatsByEndTrial() // ������ ���� ���� �� ȣ��
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
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();

        // ���� �� �θƽ� üũ
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }

    void CalcStatByLoot(S_Card hitCard) // ���ǰ� ���Ŀ� ���� ����ǰ�� ���� Ʈ���Ÿ� �ϴ� �뵵
    {
        //List<S_Card> stacks = pCard.GetStackCards();

        //// ����ǰ ���� ������(����ǰ�� ������ ��Ʈ�� ī�尡 �ƴϱ⿡ ���� �ٸ��� ���ȴ�.)
        //int count = 0;
        //bool canOverflow = false;

        //// �޾Ƹ�, ����ǰ ���� ����, ������, ������, ������ Ȯ��
        //foreach (S_Loot loot in pLoot.OwnedLoots)
        //{
        //    if (hitCard != null && loot.Condition == S_ActivationConditionEnum.Reverb)
        //    {
        //        switch (loot.Modify)
        //        {
        //            case S_ActivationModifyEnum.AnyCard:
        //                if (hitCard != null)
        //                {
        //                    await loot.ActiveLoot(this, stacks, hitCard);
        //                }
        //                break;
        //        }
        //    }
        //    else if (hitCard != null && loot.Condition == S_ActivationConditionEnum.Overflow)
        //    {
        //        count = 0;
        //        canOverflow = false;
        //        switch (loot.Modify)
        //        {
        //            case S_ActivationModifyEnum.AnyCard:
        //                foreach (var c in stacks)
        //                {
        //                    count++;
        //                }
        //                canOverflow = true;
        //                break;
        //        }


        //        if (canOverflow && count >= loot.OverflowNumber)
        //        {
        //            await loot.ActiveLoot(this, stacks, hitCard);
        //        }
        //    }
        //    else if (loot.Condition == S_ActivationConditionEnum.Loot_Condition || loot.Condition == S_ActivationConditionEnum.Loot_Growth)
        //    {
        //        await loot.ActiveLoot(this, stacks, hitCard);
        //    }
        //}

        //// ��� �� 
        //S_LootInfoSystem.Instance.UpdateLootBoardVFX();
    }

    // ������ ����
    public void ResetStackSum()
    {
        StackSum = 0;
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    public async Task GetDamagedByStand(int value)
    {
        standDamagedHealth += value;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();

        // �α� ����
        S_EffectActivator.Instance.GenerateEffectLog($"ü�� -{value}");

        // �÷��̾� �̹��� VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Health);
    }
    // ���� ���� MinMax ����
    public void CheckStatsMinMaxValue() // ü�� �� ���� ���
    {
        // ü��
        currentHealth = MaxHealth - standDamagedHealth + additionalHealth;
        if (currentHealth <= 0) // ���� ����
        {
            S_GameOverSystem.Instance.AppearGameOverPanel();
        }
        if (currentHealth > MaxHealth) // ���� ġ���� ����.
        {
            currentHealth = MaxHealth;
            additionalHealth = standDamagedHealth;
        }

        // ����
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

        // ������ �ɷ�ġ
        //if (StackSum < 0) StackSum = 0;
        if (CurrentLimit < 0) CurrentLimit = 0;
        if (CurrentGold < 0) CurrentGold = 0;
        if (CurrentStrength < 0) CurrentStrength = 0;
        if (CurrentMind < 0) CurrentMind = 0;
        if (CurrentLuck < 0) CurrentLuck = 0;
    }
    // ����
    public bool CanUseDetermination()
    {
        CheckStatsMinMaxValue();
        return currentDetermination > 0;
    }
    public void UseDetermination()
    {
        useDetermination++;
        CheckStatsMinMaxValue();
        S_StatInfoSystem.Instance.ChangeStatVFX();
    }
    // Ư�� ȿ��
    public void CheckBurstAndCleanHit()
    {
        if (StackSum > CurrentLimit) // ���� ���� �Ѱ踦 �ʰ��ߴٸ�
        {
            IsBurst = true;
            IsCleanHit = false;
        }
        else if (StackSum == CurrentLimit) // ���� ���� �Ѱ�� ���ٸ�
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
    // Get
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
    #region �ɷ�ġ ���(����� �ȵ�)
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

    public void ActiveVFXCollection() // ���� ��ġ(�ɷ�ġ, ���� ��, �Ѱ�, ������ ü��, ����ǰ ��) ��ȭ VFX
    {
        S_StatInfoSystem.Instance.ChangeStatVFX();
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_SkillInfoSystem.Instance.UpdateSkillObject();
        S_FoeInfoSystem.Instance.ChangeHealthValueVFX();
    }
}


public struct S_StatHistory // ��Ʋ�⸦ ���� ���� �ɷ�ġ �� Ư�� ���� �����ϴ� �����丮 ����ü
{
    // �� ȿ������ �߻���Ų ī��
    public S_StatHistoryTriggerEnum HistoryTrigger;
    public S_Card TriggerCard;

    // ������ ����
    public int CurrentLimit;

    // ���� �ɷ�ġ
    public int AdditionalHealth;
    public int AdditionalDetermination;
    public int CurrentGold;

    // �߰� �ɷ�ġ
    public int CurrentStrength;
    public int CurrentMind;
    public int CurrentLuck;

    // Ư�� ����
    public S_FirstEffectEnum IsFirst;
    public bool IsDelusion;
    public bool IsExpansion;

    // ����
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
public enum S_FirstEffectEnum // �켱 ȿ�� ������
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