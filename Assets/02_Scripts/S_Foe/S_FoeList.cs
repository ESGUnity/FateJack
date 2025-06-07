using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_FoeList
{
    public static List<S_Foe> Clotho = new()
    {
        new Foe_GateKeeper(), new Foe_TaboosWatcher(),

        // 시험대
    };
    public static List<S_Foe> Clotho_Elite = new()
    {
        new Foe_HypnosSleepWaker(), new Foe_OneiroiDarknessEater(),
    };
    public static List<S_Foe> Clotho_Boss = new()
    {
        new Foe_ClothoFateWeaver(), new Foe_ClothoBeginningFate(),
    };

    public static List<S_Foe> Lachesis = new()
    {
        new Foe_DeterminationEater(), new Foe_Devourer(),
    };
    public static List<S_Foe> Lachesis_Elite = new()
    {
        new Foe_OizysTheRebel(), new Foe_GerasTheWithered(),
    };
    public static List<S_Foe> Lachesis_Boss = new()
    {
        new Foe_LachesisTheDecider(), new Foe_LachesisFreedomFate(),
    };

    public static List<S_Foe> Atropos = new()
    {
        new Foe_SacredHerald(), new Foe_GraveKeeper(),
    };
    public static List<S_Foe> Atropos_Elite = new()
    {
        new Foe_MorosDoomBringer(), new Foe_ThanatosTheRest(), 
    };
    public static List<S_Foe> Atropos_Boss = new()
    {
        new Foe_AtroposTheReaper(), new Foe_AtroposTwillightFate(),
    };

    public static S_Foe GetRandomFoe(S_FoeTypeEnum foeType) // 무작위 피조물 능력을 가져오기
    {
        // 시련을 세팅
        int realTrial = S_GameFlowManager.Instance.CurrentTrial + 1;

        // 피조물 찾기
        List<S_Foe> list = new();

        switch (foeType)
        {
            case S_FoeTypeEnum.Clotho:
                list = Clotho;
                break;
            case S_FoeTypeEnum.Lachesis:
                list = Lachesis;
                break;
            case S_FoeTypeEnum.Atropos:
                list = Atropos;
                break;
            case S_FoeTypeEnum.Clotho_Elite:
                list = Clotho_Elite;
                break;
            case S_FoeTypeEnum.Lachesis_Elite:
                list = Lachesis_Elite;
                break;
            case S_FoeTypeEnum.Atropos_Elite:
                list = Atropos_Elite;
                break;
            case S_FoeTypeEnum.Clotho_Boss:
                list = Clotho_Boss;
                break;
            case S_FoeTypeEnum.Lachesis_Boss:
                list = Lachesis_Boss;
                break;
            case S_FoeTypeEnum.Atropos_Boss:
                list = Atropos_Boss;
                break;

        }

        // 피조물 생성
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex].Clone();
    }
}