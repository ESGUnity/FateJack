using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerTrinket : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public List<S_Trinket> OwnedTrinketList = new();
    public const int MAX_Trinket = 5;

    [Header("컴포넌트")]
    S_PlayerCard pCard;
    S_PlayerStat pStat;

    // 싱글턴
    static S_PlayerTrinket instance;
    public static S_PlayerTrinket Instance { get { return instance; } }

    void Awake()
    {
        // 컴포넌트 할당
        pCard = GetComponent<S_PlayerCard>();
        pStat = GetComponent<S_PlayerStat>();

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

    #region 전리품 관리
    public void InitTrinketsByStartGame()
    {
        foreach (S_Trinket loot in S_TrinketList.GetInitSkillsByStartGame())
        {
            AddTrinket(loot, S_TrinketInfoSystem.Instance.pos_OwnedTrinketBase.transform.position);
        }
    }
    public void AddTrinket(S_Trinket tri, Vector3 pos)
    {
        if (!IsFullTrinket())
        {
            OwnedTrinketList.Add(tri);
            S_TrinketInfoSystem.Instance.AddTrinketObj(tri, pos);
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("더 이상 쓸만한 물건을 추가할 수 없습니다.");
        }
    }
    public void RemoveTrinket(S_Trinket tri)
    {
        OwnedTrinketList.Remove(tri);
        S_TrinketInfoSystem.Instance.RemoveTrinketObj(tri);
    }
    public void SwapLeftTrinketIndex(S_Trinket tri)
    {
        int index = OwnedTrinketList.IndexOf(tri);

        if (index == 0)
        {
            S_Trinket firstTri = OwnedTrinketList[0];

            // 요소들을 왼쪽으로 한 칸씩 이동
            for (int i = 0; i < OwnedTrinketList.Count - 1; i++)
            {
                OwnedTrinketList[i] = OwnedTrinketList[i + 1];
            }

            // 마지막 자리에 처음 요소 넣기
            OwnedTrinketList[OwnedTrinketList.Count - 1] = firstTri;
        }
        else
        {
            // 일반적인 왼쪽 교체
            S_Trinket tempGo = OwnedTrinketList[index - 1];
            OwnedTrinketList[index - 1] = OwnedTrinketList[index];
            OwnedTrinketList[index] = tempGo;
        }
    }
    public bool IsFullTrinket()
    {
        return OwnedTrinketList.Count >= MAX_Trinket;
    }
    public List<S_Trinket> GetPlayerOwnedTrinkets()
    {
        return OwnedTrinketList.ToList();
    }
    #endregion
    #region 시련 진행에 따른 메서드
    public async Task UpdateTrinketByStartTrial()
    {
        foreach (S_Trinket tri in OwnedTrinketList)
        {
            if (tri.IsAccumulate) // 누적량이 있는 쓸만한 물건은 시련 시작 시 누적량을 능력치에 더해준다.
            {
                await S_EffectActivator.Instance.AddOrSubtractBattleStat(null, tri.Stat, tri.TotalTrialAccumulateValue);
            }

            // 시련 시작 시 효과를 발동한다.
            await S_EffectActivator.Instance.ActivateTrinketByStartTrial();
        }
    }
    public void UpdateTrinketByStartNewTurn()
    {
        foreach (S_Trinket tri in OwnedTrinketList)
        {
            // 매 턴마다 능력치가 변경되는 효과의 능력치를 바꿔주기
            if (tri.Effect == S_TrinketEffectEnum.Harm_TwoStat_Random)
            {
                List<S_BattleStatEnum> stat = new() { S_BattleStatEnum.Str_Mind, S_BattleStatEnum.Str_Luck, S_BattleStatEnum.Mind_Luck };
                tri.Stat = stat[Random.Range(0, stat.Count)];
            }

            // 한 턴에 ~ 하는 효과의 조건 충족 여부를 무조건 false로 바꾸기
            if (tri.Condition == S_TrinketConditionEnum.Only || tri.Condition == S_TrinketConditionEnum.Resection_Three || tri.Condition == S_TrinketConditionEnum.Overflow_Six ||
                tri.Condition == S_TrinketConditionEnum.GrandChaos_One || tri.Condition == S_TrinketConditionEnum.GrandChaos_Two)
            {
                tri.ActivatedCount = 0;
                tri.IsMeetCondition = false;
            }
        }

        // 조건 검사 한 번 해주기
        S_EffectActivator.Instance.CheckTrinketMeetCondition();

        // ActivatedCount와 IsMeetCondition이 변경되었음으로 상태 업데이트
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();
    }
    public void UpdateTrinketByEndTrial()
    {
        foreach (S_Trinket tri in OwnedTrinketList)
        {
            // 초기화
            tri.ActivatedCount = 0;
            tri.IsMeetCondition = false;
            tri.ExpectedValue = 0;

            // 누적량 고정
            if (tri.IsAccumulate)
            {
                tri.TotalTrialAccumulateValue = tri.CurrentAccumulateValue;
            }
        }

        // ActivatedCount와 IsMeetCondition이 변경되었음으로 상태 업데이트
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();
    }
    #endregion
    #region 보조 메서드
    public bool IsPlayerHavePassive(S_TrinketPassiveEnum passive, out S_Trinket trinket) // 플레이어가 보유한 쓸만한 물건 중 passive가 있는지 없는치 확인하는 메서드
    {
        foreach (S_Trinket tri in OwnedTrinketList)
        {
            if (tri.Passive == passive)
            {
                trinket = tri;
                return true;
            }
        }

        trinket = null;
        return false;
    }
    public string GetTrinketDescription(S_Trinket trinket) // 설명에 추가로 붙은 것들 메서드
    {
        StringBuilder sb = new();

        sb.Append(trinket.Description);

        // 무작위 능력치 2개
        if (trinket.Effect == S_TrinketEffectEnum.Harm_TwoStat_Random)
        {
            switch (trinket.Stat)
            {
                case S_BattleStatEnum.Str_Mind:
                    sb.Replace("능력치 2개를", "힘과 정신력을");
                    break;
                case S_BattleStatEnum.Str_Luck:
                    sb.Replace("능력치 2개를", "힘과 행운을");
                    break;
                case S_BattleStatEnum.Mind_Luck:
                    sb.Replace("능력치 2개를", "정신력과 행운을");
                    break;
            }
        }

        // 조건에 따라 추가 설명
        switch (trinket.Condition)
        {
            case S_TrinketConditionEnum.Reverb_Two:
                sb.Append($"\n(낸 카드 : {trinket.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.Reverb_Three:
                sb.Append($"\n(낸 카드 : {trinket.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.Precision_Six:
                sb.Append($"\n(스택의 카드 : {trinket.ActivatedCount}장)");
                break;
            case S_TrinketConditionEnum.Legion_Twenty:
                sb.Append($"\n(스택의 카드 무게 합 : {trinket.ActivatedCount})");
                break;
            case S_TrinketConditionEnum.Resection_Three:
                sb.Append($"\n(이번 턴에 낸 카드 : {trinket.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.Overflow_Six:
                sb.Append($"\n(이번 턴에 낸 카드 : {trinket.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.GrandChaos_One:
                sb.Append($"\n(만족한 카드 타입 : {trinket.ActivatedCount}개)");
                break;
            case S_TrinketConditionEnum.GrandChaos_Two:
                sb.Append($"\n(만족한 카드 타입 : {trinket.ActivatedCount}개)");
                break;
        }

        // 예상 값 추가
        switch (trinket.Effect)
        {
            case S_TrinketEffectEnum.Harm:
                sb.Append($"\n(예상 피해량 : {trinket.ExpectedValue})");
                break;
            case S_TrinketEffectEnum.Harm_TwoStat_Random:
                sb.Append($"\n(예상 피해량 : {trinket.ExpectedValue})");
                break;
            case S_TrinketEffectEnum.Harm_AllStat:
                sb.Append($"\n(예상 피해량 : {trinket.ExpectedValue})");
                break;
            case S_TrinketEffectEnum.Stat_Multi:
                sb.Append($"\n(예상 증가량 : {trinket.ExpectedValue})");
                break;
        }

        // 누적량이 존재하는 쓸만한 물건
        if (trinket.IsAccumulate)
        {
            sb.Append($"누적량 : {trinket.CurrentAccumulateValue}");
        }

        return sb.ToString();
    }
    #endregion
}
public enum S_TrinketConditionEnum
{
    None,
    Always,
    StartTrial,
    Reverb_One,
    Reverb_Two,
    Reverb_Three,
    Only,
    Precision_Six,
    Legion_Twenty,
    Resection_Three,
    Overflow_Six,
    GrandChaos_One,
    GrandChaos_Two,
    // 적
    GrandChaos_One_Flip,
}
public enum S_TrinketModifyEnum
{
    None, 
    Any,
    Str, Mind, Luck, Common,
}
public enum S_TrinketPassiveEnum
{
    None,
    // 공통
    CurseStr,
    CurseMind,
    CurseLuck,
    CurseCommon,
    // 쓸만한 물건
    CurseGet10Str,
    Perfect15Mind,
    Perfect8StrLuck,
    Gen1Trig,
    LuckPer1LuckTrig,
    LuckPerUnleashTrig,
    Multi20Harm,
    FirstCard2Trig,
    NoStrMindLuck1Trig,
    NoMindStrLuck1Trig,
    NoLuckStrMind1Trig,
    SameStrCommon,
    SameMindLuck,
    NoBurstPerfect,
    AddProductCount,
    // 적
    NoDeterminationDeathAttack,
    Ignore25PerHarm,
    Immunity30PerHarm,
}
public enum S_TrinketEffectEnum
{
    None,
    Harm,
    Harm_TwoStat_Random,
    Harm_AllStat,
    Stat,
    Stat_Curse,
    Stat_Multi,
    Weight,
    Limit,
    Expansion,
    First,
    ColdBlood,
    Gen,
    Gen_Deck,
    Retrigger_CurrentTurn,
    Health,
    // Foe
    DeathAttack,
    Delusion,
    CurseCurrentTurn,
}