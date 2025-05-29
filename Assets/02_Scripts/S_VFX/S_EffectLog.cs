using TMPro;
using UnityEngine;

public class S_EffectLog : MonoBehaviour
{
    [SerializeField] public TMP_Text text_EffectContent;

    void Awake()
    {
        text_EffectContent.raycastTarget = false;
    }

    public void SetEffectText(string text)
    {
        text_EffectContent.text = text;
    }

    //public void SetEffectText(S_CardBasicEffectEnum basicEffect, S_BattleStatEnum stat = S_BattleStatEnum.None, int value = 0, S_CardSuitEnum suit = S_CardSuitEnum.None)
    //{
    //    switch (basicEffect)
    //    {
    //        case S_CardBasicEffectEnum.Increase_Strength: text_EffectContent.text = $"힘 +5"; break;
    //        case S_CardBasicEffectEnum.Increase_Mind: text_EffectContent.text = $"정신력 +5"; break;
    //        case S_CardBasicEffectEnum.Increase_Luck: text_EffectContent.text = $"행운 +5"; break;
    //        case S_CardBasicEffectEnum.Increase_AllStat: text_EffectContent.text = $"모든 능력치 +5"; break;
    //        case S_CardBasicEffectEnum.Break_Zenith: 
    //            if (stat == S_BattleStatEnum.Strength) text_EffectContent.text = $"힘 +{value}";
    //            if (stat == S_BattleStatEnum.Mind) text_EffectContent.text = $"정신력 +{value}";
    //            if (stat == S_BattleStatEnum.Luck) text_EffectContent.text = $"행운 +{value}";
    //            break;
    //        case S_CardBasicEffectEnum.Break_Genesis:
    //            if (stat == S_BattleStatEnum.Strength) text_EffectContent.text = $"힘 +{value}";
    //            if (stat == S_BattleStatEnum.Mind) text_EffectContent.text = $"정신력 +{value}";
    //            if (stat == S_BattleStatEnum.Luck) text_EffectContent.text = $"행운 +{value}";
    //            break;
    //        case S_CardBasicEffectEnum.Manipulation: text_EffectContent.text = $"숫자 합 -2"; break;
    //        case S_CardBasicEffectEnum.Manipulation_Cheat: text_EffectContent.text = $"숫자 합 -{value}"; break;
    //        case S_CardBasicEffectEnum.Manipulation_Judge: text_EffectContent.text = $"숫자 합 -{value}"; break;
    //        case S_CardBasicEffectEnum.Resistance: text_EffectContent.text = $"한계 +1"; break;
    //        case S_CardBasicEffectEnum.Resistance_Indomitable: text_EffectContent.text = $"한계 +{value}"; break;

    //        case S_CardBasicEffectEnum.Harm_Strength: text_EffectContent.text = $"피해 : {value}"; break;
    //        case S_CardBasicEffectEnum.Harm_Mind: text_EffectContent.text = $"피해 : {value}"; break;
    //        case S_CardBasicEffectEnum.Harm_Luck: text_EffectContent.text = $"피해 : {value}"; break;
    //        case S_CardBasicEffectEnum.Harm_StrengthAndMind: text_EffectContent.text = $"피해 : {value}"; break;
    //        case S_CardBasicEffectEnum.Harm_StrengthAndLuck: text_EffectContent.text = $"피해 : {value}"; break;
    //        case S_CardBasicEffectEnum.Harm_MindAndLuck: text_EffectContent.text = $"피해 : {value}"; break;
    //        case S_CardBasicEffectEnum.Harm_Carnage: text_EffectContent.text = $"피해 : {value}"; break;

    //        case S_CardBasicEffectEnum.Tempering: text_EffectContent.text = $"의지 +1"; break;
    //        case S_CardBasicEffectEnum.Plunder: text_EffectContent.text = $"골드 +2"; break;
    //        case S_CardBasicEffectEnum.Plunder_Raid: text_EffectContent.text = $"골드 +{value}"; break;
    //        case S_CardBasicEffectEnum.Creation_Anarchy: text_EffectContent.text = $"창조"; break;
    //        case S_CardBasicEffectEnum.Creation_SameSuit: text_EffectContent.text = $"창조"; break;
    //        case S_CardBasicEffectEnum.Creation_SameNumber: text_EffectContent.text = $"창조"; break;
    //        case S_CardBasicEffectEnum.Creation_PlethoraNumber: text_EffectContent.text = $"창조"; break;
    //        case S_CardBasicEffectEnum.AreaExpansion: text_EffectContent.text = $"전개 +1"; break;

    //        case S_CardBasicEffectEnum.First_SameSuit: 
    //            if (suit == S_CardSuitEnum.Spade) text_EffectContent.text = $"우선 : 스페이드";
    //            if (suit == S_CardSuitEnum.Heart) text_EffectContent.text = $"우선 : 하트";
    //            if (suit == S_CardSuitEnum.Diamond) text_EffectContent.text = $"우선 : 다이아몬드";
    //            if (suit == S_CardSuitEnum.Clover) text_EffectContent.text = $"우선 : 클로버";
    //            break;
    //        case S_CardBasicEffectEnum.First_LeastSuit: text_EffectContent.text = $"우선 : 가장 적은 문양"; break;
    //        case S_CardBasicEffectEnum.First_SameNumber: text_EffectContent.text = $"우선 : 숫자 {value}"; break;
    //        case S_CardBasicEffectEnum.First_CleanHitNumber: text_EffectContent.text = $"우선 : 클린히트 숫자"; break;
    //        case S_CardBasicEffectEnum.Undertow: text_EffectContent.text = $"역류"; break;
    //        case S_CardBasicEffectEnum.Guidance_NoSuit: text_EffectContent.text = $"인도"; break;
    //        case S_CardBasicEffectEnum.Guidance_NoNumber: text_EffectContent.text = $"인도"; break;
    //        case S_CardBasicEffectEnum.GrandGuidance_LeastSuit: text_EffectContent.text = $"인도"; break;
    //    }
    //}
    //public void SetEffectText(S_CardAdditiveEffectEnum additiveEffect, S_BattleStatEnum stat = S_BattleStatEnum.None, int value = 0)
    //{
    //    switch (additiveEffect)
    //    {
    //        case S_CardAdditiveEffectEnum.Enhancement:
    //            if (stat == S_BattleStatEnum.Strength) text_EffectContent.text = $"힘 +{value}";
    //            if (stat == S_BattleStatEnum.Mind) text_EffectContent.text = $"정신력 +{value}";
    //            if (stat == S_BattleStatEnum.Luck) text_EffectContent.text = $"행운 +{value}";
    //            break;
    //        case S_CardAdditiveEffectEnum.Robbery:
    //            text_EffectContent.text = $"골드 +3";
    //            break;
    //        case S_CardAdditiveEffectEnum.Omen:
    //            text_EffectContent.text = $"골드 +1";
    //            break;
    //    }
    //}
    //public void SetEffectText(S_CardDebuffConditionEnum debuff)
    //{
    //    switch (debuff)
    //    {
    //        case S_CardDebuffConditionEnum.Breakdown:
    //            text_EffectContent.text = $"제외됨!";
    //            break;
    //        case S_CardDebuffConditionEnum.Paranoia:
    //            text_EffectContent.text = $"망상";
    //            break;
    //        case S_CardDebuffConditionEnum.Spell:
    //            text_EffectContent.text = $"저주받음!";
    //            break;
    //        case S_CardDebuffConditionEnum.Rebel:
    //            text_EffectContent.text = $"한계 -1";
    //            break;
    //    }
    //}
}
