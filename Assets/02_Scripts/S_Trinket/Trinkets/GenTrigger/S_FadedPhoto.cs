using UnityEngine;

public class S_FadedPhoto : S_Trinket
{
    public S_FadedPhoto() : base
    (
        "FadedPhoto",
        "빛바랜 사진",
        "한 턴에 카드를 3장 이하만 냈다면 이번 턴에 낸 카드는 효과를 1번 더 발동합니다.",
        1,
        0,
        S_TrinketConditionEnum.Resection_Three,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Retrigger_CurrentTurn,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_FadedPhoto();
    }
}