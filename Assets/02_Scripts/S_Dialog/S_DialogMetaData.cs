using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class S_DialogMetaData
{
    static Dictionary<int, string> storeName = new()
    {
        { 0, "데어 데달로스" },
        { 1, "데런 데달로스" },
        { 2, "데이브 데달로스" },
        { 3, "데킨 데달로스" },
        { 4, "브룬데 데달로스" },
        { 5, "데이 데달로스" },
        { 6, "데토난 데달로스" },
        { 7, "데인 데달로스" },
        { 8, "데락 데달로스" },
        { 9, "다이데 데달로스" },
        { 10, "데브 데달로스" },
        { 11, "데르노 데달로스" },
        { 12, "데쿠 데달로스" },
        { 13, "퓨데 데달로스" },
        { 14, "리니데 데달로스" },
        { 15, "데칼 데달로스" },
        { 16, "데스롤 데달로스" },
        { 17, "데자와 데달로스" },
        { 18, "만데 데달로스" },
        { 19, "데산히억 데달로스" },
        { 20, "쟝데 데달로스" },
        { 21, "아이데 데달로스" },
    };
    static Dictionary<string, string> storeMonologs = new Dictionary<string, string>()
    {
        // 데달로스
        { "Store_Intro", "상점에 오신 걸 환영합니다! 마음껏 둘러보세요." },
        { "Store_NotEnoughGold", "골드가 부족합니다." },
        { "Store_FullTrinket", "쓸만한 물건이 가득 찼습니다." },
        { "Store_EmptyTrinket", "제거할 쓸만한 물건이 없습니다." },


        { "Store_GateKeeper", "문지기는 한 턴에 카드를 3장 이하만 냈다면 망상을 거는 적입니다. 아무래도 카드를 많이 내는 편이 좋을 것 같군요.." },
        { "Store_HypnosSleepWaker", "잠을 깨는 히프노스는 한 턴에 카드를 3장 이하만 냈다면 이번 턴에 낸 카드를 모두 저주하는 적입니다. 아무래도 카드를 많이 내는 편이 좋을 것 같군요.." },
        { "Store_ClothoFateWeaver", "운명을 짓는 자 클로토께서는 한 턴에 카드를 3장 이하만 냈다면 당신을 공격할 때 즉시 처치하십니다. 아무래도 카드를 많이 내는 편이 상대하기 좋을 것 같군요.." },
        { "Store_Devourer", "탐식자는 공용 카드를 낼 때마다 모든 능력치를 2 잃게 되는 적입니다. 카드를 낼 때 신중해야겠군요.." },
        { "Store_OizysTheRebel", "격동하는 오이지스는 당신의 의지가 0이라면 공격 시 즉시 처치하는 적입니다. 되돌리기를 사용할 때 신중해야겠군요.." },
        { "Store_LachesisTheDecider", "정하는 자 라케시스께서는 공용 카드를낼 때마다 그 공용 카드를 저주하십니다. 카드를 낼 때 신중해야겠군요.." },
        { "Store_GraveKeeper", "묘지기는 힘 카드를 3장 낼 때마다 망상을 거는 적입니다. 카드를 낼 때 신중해야겠군요.." },
        { "Store_MorosDoomBringer", "파멸을 부르는 모로스는 최대 체력의 25%를 초과한 피해를 무시하는 적입니다. 여러 번 나누어 피해를 주는 편이 좋겠네요." },
        { "Store_ThanatosTheRest", "안식의 타나토스는 최대 체력의 30%보다 작은 피해에는 체력을 잃지 않는 적입니다. 한 번에 강한 피해를 주어야겠군요." },
        { "Store_AtroposTheReaper", "거두는 자 아트로포스께서는 한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 1장 이상 내지 못했다면 당신을 공격할 때 즉시 처치하십니다. 카드를 내는 게 강제되는만큼 신중하셔야합니다." },
    };
    static Dictionary<string, DialogData> dialogDatas = new Dictionary<string, DialogData>()
    {
        // 튜토리얼 : 인트로
        { "Tutorial_Intro_1", new DialogData("???", "반가워. 용케도 이곳에 당도했군.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_2", new DialogData("???", "이곳은 인간들의 운명을 만들고 관리하는 신들의 세계야. 어쩌면 천국이나 지옥, 저승과도 같은 곳이려나.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_3", new DialogData("???", "너가 이곳에 온 이유는.. 아마 너가 제일 알겠지. 너는 곧 죽을 운명이니깐.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_4", new DialogData("???", "이곳의 권력을 쥐고있는 운명의 세 여신이 정한 운명대로 모든 인간은 태어나고 살다가 끝을 맞이해.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_5", new DialogData("???", "그런데 간혹 너처럼 운명에 저항하고자 하는 의지가 강한 인간은 운명을 초월하기 위해 이곳까지 오게 돼. 즉 너같은 존재는 신의 실수라는 거지.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_6", new DialogData("???", "하지만 신에겐 실수란 없어. 어쩌면 너가 여기 온 것도 운명일 수 있다는 거지.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_7", new DialogData("???", "서론이 길었네. 그래서 너가 운명을 초월하려면 인간의 운명을 결정짓는 운명의 세 여신인 클로토, 라케시스, 아트로포스를 만나 네 의지를 증명해야해.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_8", new DialogData("???", "그러나 쉽지는 않을거야. 운명의 세 여신을 만나기 전에 운명을 수호하는 존재들이 너를 막아설테니깐.", S_ActivateBtnEnum.Btn_Next) },

        // 튜토리얼 : 카드 내기
        { "Tutorial_Hit_1", new DialogData("???", "그럼 너는 그런 존재들에 대해 어떻게 의지를 증명하고 대항할 수 있을까?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_2", new DialogData("???", "그건 바로 네 운명인 카드야. 너가 보유하고 있는 카드를 내서 네 운명을 네 스스로 만들어나가, 너를 막아서는 존재에 대항할 운명을 직접 만드는거야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_3", new DialogData("???", "카드를 낼 땐 2장의 보기 중 하나를 선택해서 낼 수 있어.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_4", new DialogData("???", "그럼 카드 내기 버튼을 눌러 카드를 내봐.", S_ActivateBtnEnum.Btn_Hit) },

        // 튜토리얼 : 카드에 대해서
        { "Tutorial_Card_1", new DialogData("???", "잘했어. 그렇게 카드를 낼 수 있어.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_2", new DialogData("???", "방금 너가 카드를 낸 곳을 스택이라고 하고, 아직 내지 않은 카드가 있는 곳을 덱이라고 해.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_3", new DialogData("???", "이어서 카드에 대해 설명해줄게.", S_ActivateBtnEnum.Obj_StackCards) },
        { "Tutorial_Card_4", new DialogData("???", "기본적으로 카드는 낸 후에, 너가 스탠드를 하면 효과가 발동해. 이때 왼쪽부터 오른쪽으로 너가 낸 순서대로 발동돼.", S_ActivateBtnEnum.Obj_StackCards) },
        { "Tutorial_Card_5", new DialogData("???", "카드의 효과는 크게 힘 카드, 정신력 카드, 행운 카드, 공용 카드로 나뉘어져. 카드의 중앙 하단을 보면 어떤 유형의 카드인지 알 수 있어.", S_ActivateBtnEnum.Obj_StackCards) },
        { "Tutorial_Card_6", new DialogData("???", "각 카드는 너의 주요 능력치인 힘, 정신력, 행운과 관련이 있고 왼쪽 하단에서 확인할 수 있어.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_7", new DialogData("???", "이어서 카드의 왼쪽 상단의 숫자는 해당 운명에 대해 너가 짊어져야할 무게야.", S_ActivateBtnEnum.Obj_StackCards) },
        { "Tutorial_Card_8", new DialogData("???", "아래쪽에 게이지가 하나 보일거야. 그게 바로 한 턴에 너가 짊어질 수 있는 무게야. 카드를 낼 때 무게를 얻게 돼.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_9", new DialogData("???", "그리고 한계가 너가 짊어질 수 있는 최대 무게야. 만약 너가 카드를 냈을 때 저 한계를 넘지 않는다면 큰 상관이 없어.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_10", new DialogData("???", "또한 무게와 한계가 같아지면 완벽이라는 상태를 얻을 수 있고 너가 주는 모든 피해가 2배 증가하게 돼.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_11", new DialogData("???", "그러나 무게가 한계를 넘게 되면 버스트라는 상태를 얻게 되고 너가 주는 모든 피해가 0.25배 감소하게 돼. 그리고 스탠드 외에 다른 행동은 할 수 없어. 그러니 카드를 낼 때 신중해야겠지.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_12", new DialogData("???", "그럼 왼쪽 하단의 카드 더미를 눌러서 너가 보유한 덱을 보고 와. 덱에서 스택으로 돌아오려면 다시 카드 더미를 누르면 돼.", S_ActivateBtnEnum.Obj_ViewDeck) },

        // 튜토리얼 : 되돌리기
        { "Tutorial_Twist_1", new DialogData("???", "다음은 되돌리기야. 왼쪽 하단의 의지를 1 소모해서 너가 한 턴 동안 낸 카드를 모두 되돌릴 수 있어.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Twist_2", new DialogData("???", "단 버스트 시에는 할 수 없으니까 유의해.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Twist_3", new DialogData("???", "그럼 되돌리기 버튼을 눌러서 다시 이번 턴의 처음 상태로 되돌아가봐.", S_ActivateBtnEnum.Btn_Twist) },

        // 튜토리얼 : 스탠드
        { "Tutorial_Stand_1", new DialogData("???", "잘했어. 낼 수 있는 카드가 제한적이고 순서도 중요하니깐 마음에 들지 않으면 되돌려서 다시 낼 수 있으니 유용하게 쓸 수 있을거야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Stand_2", new DialogData("???", "이어서 스탠드에 대해서 알려줄게.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Stand_3", new DialogData("???", "스탠드를 누르면 스택에 있는 카드의 효과가 발동하고 턴이 종료돼. 그리고 네 무게도 다시 0으로 초기화 돼.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Stand_4", new DialogData("???", "스택에 있는 카드의 효과가 모두 발동하고도 네 적이 살아있다면 적은 너를 공격할거야. 그럼 네 왼쪽 하단에 있는 체력을 1 잃게 돼.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Stand_5", new DialogData("???", "네 체력이 0이 되면 패배하게 돼. 즉 죽음이자 원래 운명을 받아들여야 되는 거지.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Stand_6", new DialogData("???", "그러니 체력이 0이 되기 전에 너를 막으려는 적을 먼저 쓰러뜨려야 해.", S_ActivateBtnEnum.UI_FoeStat) },

        // 튜토리얼 : 쓸만한 물건
        { "Tutorial_Trinket_1", new DialogData("???", "마지막으로 쓸만한 물건에 대해서 알려줄게. 네 오른쪽 하단에 있는 것이 쓸만한 물건이야.", S_ActivateBtnEnum.Obj_Trinket) },
        { "Tutorial_Trinket_2", new DialogData("???", "쓸만한 물건은 네 덱을 보조할거야. 네 덱에 따라 적절한 물건을 찾는다면 더 큰 시너지를 낼 수도 있을거야.", S_ActivateBtnEnum.Obj_Trinket) },
        { "Tutorial_Trinket_3", new DialogData("???", "쓸만한 물건도 스탠드 시 발동하지만 몇몇 물건은 카드를 낼 때 발동하거나 지속적으로 이로운 효과를 주기도 해.", S_ActivateBtnEnum.Obj_Trinket) },
        { "Tutorial_Trinket_4", new DialogData("???", "쓸만한 물건은 적을 쓰러뜨린 후에 갈 수 있는 상점에서 획득할 수 있어.", S_ActivateBtnEnum.Obj_Trinket) },
        { "Tutorial_Trinket_5", new DialogData("???", "카드 역시 상점에서 얻거나 기존 카드의 효과를 변경할 수 있어.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Trinket_6", new DialogData("???", "설명은 여기까지야. 나머진 너가 시련을 겪어가며 알아갈 수 있을거야.", S_ActivateBtnEnum.Obj_Trinket) },

        // 튜토리얼 : 아웃트로
        { "Tutorial_Outro_1", new DialogData("라케시스", "아, 그리고 나는 너를 막아설 이곳에서 가장 강력한 존재, 운명의 세 여신 중 인간이 어떤 삶을 살지 그 운명을 결정짓는 라케시스야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_2", new DialogData("라케시스", "네 의지의 강함을 계속 증명해가다보면 내가 널 왜 도와줬는지 알 수 있겠지.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_3", new DialogData("라케시스", "그럼 나는 이만 가보도록 할게! 앞으로 쭉 가면 상점이 나올거야. 그리고 상점을 나서면 너를 막으려는 존재들이, 너의 인생에서 가장 어려운 시련이 기다리겠지.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_4", new DialogData("라케시스", "힘내. 또 볼 수 있었으면 좋겠네.", S_ActivateBtnEnum.Btn_Next) },

        // 튜토리얼 : 상점
        { "Tutorial_Store_1", new DialogData("데어 데달로스", "반가워요! 상점에 오신 걸 환영합니다. 그런데 당신은...", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Store_2", new DialogData("데어 데달로스", "그렇군요.. 운명을 초월하기 위해 이곳에 당도한 인간..", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Store_3", new DialogData("데어 데달로스", "아, 저는 당신을 막으려는 존재가 아닙니다. 라케시스 님의 명령에 따라 당신과 같은 인간을 돕는 일개 상인이죠.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Store_4", new DialogData("데어 데달로스", "그럼 상점을 한 번 둘러보시겠어요? 상품은 항상 무료로 제공하는 무료 상품과 골드를 주고 구매하셔야하는 유료 상품으로 구분됩니다.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Store_5", new DialogData("데어 데달로스", "아, 그리고 카드에 대해서 조금 더 말씀드릴 부분이 있습니다. 바로 카드의 효과를 보조하는 각인입니다.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Store_6", new DialogData("데어 데달로스", "카드에는 최대 1개의 각인을 새길 수가 있습니다. 각인은 이 상점에서만 부여하거나 제거할 수 있습니다.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Store_7", new DialogData("데어 데달로스", "각인은 발동 조건이 붙는 대신 무게를 감소시켜주거나, 효과를 강화하는 등 여러 부가 효과를 부여해줍니다.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Store_8", new DialogData("데어 데달로스", "아직 시련을 겪지 않으셨으니 이번에만 유료 상품 하나를 무료로 드리겠습니다. 유료 상품 하나를 선택해주세요!", S_ActivateBtnEnum.Btn_Next) },
    };

    public static string GetMerchantName(int trial)
    {
        return storeName[trial];
    }
    public static string GetStoreMonologData(string key)
    {
        if (storeMonologs.ContainsKey(key))
        {
            return storeMonologs[key];
        }

        return $"[Missing Dialogue: {key}]";
    }
    public static DialogData GetDialogData(string key)
    {
        if (dialogDatas.ContainsKey(key))
        {
            return dialogDatas[key];
        }

        Debug.Log("S_DialogMetaData Send : 에러");
        return default;
    }
    public static List<DialogData> GetDialogsByPrefix(string prefix)
    {
        return dialogDatas
            .Where(pair => pair.Key.StartsWith(prefix))
            .OrderBy(pair => GetNumberSuffix(pair.Key)) // 숫자 기준 정렬
            .Select(pair => pair.Value)
            .ToList();
    }
    // 키 끝의 숫자를 정수로 파싱
    private static int GetNumberSuffix(string key)
    {
        // 예: "Tutorial_Intro_5" → 5
        var match = Regex.Match(key, @"_(\d+)$");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}
