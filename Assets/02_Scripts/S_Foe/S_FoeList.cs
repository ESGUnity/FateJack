using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_FoeList
{
    public static List<S_Foe> Foes = new()
    {
        new S_GateKeeper(), 
        new S_HypnosSleepWaker(),
        new S_ClothoFateWeaver(),

        new S_Devourer(), 
        new S_OizysTheRebel(), 
        new S_LachesisTheDecider(),

        new S_GraveKeeper(),
        new S_MorosDoomBringer(), new S_ThanatosTheRest(), 
        new S_AtroposTheReaper(), 
    };

    public static S_Foe GetRandomFoe(S_FoeTypeEnum foeType) // 무작위 피조물 능력을 가져오기
    {
        // 시련을 세팅
        int realTrial = S_GameFlowManager.Instance.CurrentTrial + 1;

        // 적 타입에 맞는 리스트 생성
        List<S_Foe> list = Foes.ToList().Where(x => x.FoeType == foeType).ToList();

        // 리스트에서 무작위로 고르기
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex].Clone();
    }
}