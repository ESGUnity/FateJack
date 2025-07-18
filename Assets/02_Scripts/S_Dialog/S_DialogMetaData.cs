using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class S_DialogMetaData
{
    public static Dictionary<string, DialogData> Monologs = new Dictionary<string, DialogData>()
    {
        // 히트 관련
        { "Hit_BurstOrPerfect", new DialogData("???", "버스트나 완벽 시엔 카드를 낼 수 없다고!!!", default) },
        { "Hit_NoCardInDeckAndUsed", new DialogData("???", "더 이상 낼 카드가 없는데?", default) },

        // 보상
        { "Reward_Start", new DialogData("???", "자 맘껏 골라봐!", default) },
        { "Reward_NoProduct", new DialogData("???", "더 이상 고를게 없어! 뭘 뜸들이는거야?", default) },

        // 구매 전 포인터 올려놨을 때 대사
        { "Reward_OldMold", new DialogData("???", "그건 낡은 거푸집이야! 무작위 카드 5장 중에서 1장을 네 멋있는 덱에 넣을 수 있어!", default) },
        { "Reward_MeltedMold", new DialogData("???", "그건 녹아내린 거푸집이야! 엄청 뜨겁네!! 무작위 힘 카드 3장 중에서 1장을 고르게 해줄게!", default) },
        { "Reward_SpiritualMold", new DialogData("???", "오오.. 그건 영험한 거푸집! 무작위 정신력 카드 3장 중에서 1장을 네 덱에 추가할 수 있어!", default) },
        { "Reward_BrightMold", new DialogData("???", "으앗 눈부셔!! 빛나는 거푸집이다! 무작위 행운 카드 3장 중에서 1장이 너와 함께 할거야.", default) },
        { "Reward_DelicateMold", new DialogData("???", "그건 정교한 거푸집이야. 무작위 공용 카드 3장 중에서 1장이 네 덱에 참여할거야.", default) },
        { "Reward_Immunity", new DialogData("???", "그걸 고르면 네 무작위 카드 10장 중 1장에 <Accent_Basic>면역</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>면역</Accent_Basic>이 새겨진 카드는 절대로 <Accent_Basic>저주</Accent_Basic>받지않아!", default) },
        { "Reward_QuickAction", new DialogData("???", "그걸 고르면 네 무작위 카드 10장 중 1장에 <Accent_Basic>속전속결</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>속전속결</Accent_Basic>이 새겨진 카드는 시련이 시작하자마자 곧장 필드로 튀어나올거야!", default) },
        { "Reward_Rebound", new DialogData("???", "그걸 고르면 네 무작위 카드 10장 중 1장에 <Accent_Basic>메아리</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>메아리</Accent_Basic>가 새겨진 카드는 <Accent_Basic>발현</Accent_Basic> 효과가 1번 더 발동해!!", default) },
        { "Reward_Fix", new DialogData("???", "그걸 고르면 무작위 카드 10장 중 1장에 <Accent_Basic>고정</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>고정</Accent_Basic>이 새겨진 카드는 스탠드를 해도 필드에 남아있어!!", default) },
        { "Reward_Flexible", new DialogData("???", "그걸 고르면 무작위 카드 10장 중 1장에 <Accent_Basic>유연</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>유연</Accent_Basic>이 새겨진 카드는 스탠드 직후에 필드의 가장 오른쪽으로 이동해! 아! 왼쪽으로 이동하는 <Accent_Basic>도약</Accent_Basic>이랑 헷갈리지마!", default) },
        { "Reward_Leap", new DialogData("???", "그걸 고르면 무작위 카드 10장 중 1장에 <Accent_Basic>도약</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>도약</Accent_Basic>이 새겨진 카드는 스탠드 직후에 필드의 가장 왼쪽으로 이동해! 아! 오른쪽으로 이동하는 <Accent_Basic>유연</Accent_Basic>이랑 헷갈리지마!", default) },
        { "Reward_Dismantle", new DialogData("???", "그걸 고르면 무작위 카드 10장 중 1장에 <Accent_Basic>분해</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>분해</Accent_Basic>가 새겨진 카드는 <Accent_Basic>무게</Accent_Basic>가 사라져!!", default) },
        { "Reward_Mask", new DialogData("???", "그걸 고르면 무작위 카드 10장 중 1장에 <Accent_Basic>가면</Accent_Basic> 각인을 새겨줄거야! <Accent_Basic>가면</Accent_Basic>이 새겨진 카드는 <Accent_Basic>힘 카드</Accent_Basic>, <Accent_Basic>정신력 카드</Accent_Basic>, <Accent_Basic>행운 카드</Accent_Basic>, <Accent_Basic>공용 카드</Accent_Basic> 모두가 될 수 있어!!", default) },

        // 구매 후 무엇을 고르는건지 알려주는 대사
        { "Reward_OldMold_Buied", new DialogData("???", "무작위 카드 5장이야! 1장만 고를 수 있어!", default) },
        { "Reward_MeltedMold_Buied", new DialogData("???", "여기 <Accent_Basic>힘 카드</Accent_Basic> 3장이야! 빨리 1장 골라봐!!!", default) },
        { "Reward_SpiritualMold_Buied", new DialogData("???", "여기 <Accent_Basic>정신력 카드</Accent_Basic> 3장이야. 딱 1장만 고를 수 있으니 신중하게 선택해!", default) },
        { "Reward_BrightMold_Buied", new DialogData("???", "여기 <Accent_Basic>행운 카드</Accent_Basic> 3장이야. 어떤 카드가 네 덱에 들어가게 될까?", default) },
        { "Reward_DelicateMold_Buied", new DialogData("???", "여기 <Accent_Basic>공용 카드</Accent_Basic> 3장이야. 다는 안돼! 1장만 골라~", default) },
        { "Reward_Immunity_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>면역</Accent_Basic> 각인을 새겨줄까? 누가 저주에 걸리지 않는 멋있는 카드가 될까?", default) },
        { "Reward_QuickAction_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>속전속결</Accent_Basic> 각인을 새겨줄까? 빨리 골라봐!", default) },
        { "Reward_Rebound_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>메아리</Accent_Basic> 각인을 새겨줄까? 어떤 카드에 <Accent_Basic>메아리</Accent_Basic> 각인을 새겨줄까!!!", default) },
        { "Reward_Fix_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>고정</Accent_Basic> 각인을 새겨줄까? 필드에 남아 너를 끝까지 지켜줄 카드말이야!", default) },
        { "Reward_Flexible_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>유연</Accent_Basic> 각인을 새겨줄까? 오른쪽으로 이동할 카드!!", default) },
        { "Reward_Leap_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>도약</Accent_Basic> 각인을 새겨줄까? 왼쪽으로 이동할 카드!!", default) },
        { "Reward_Dismantle_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>분해</Accent_Basic> 각인을 새겨줄까? <Accent_Basic>무게</Accent_Basic> 부분을 찢어줄게!", default) },
        { "Reward_Mask_Buied", new DialogData("???", "어떤 카드에 <Accent_Basic>가면</Accent_Basic> 각인을 새겨줄까? 페르소나!!!", default) },

        // 카드 선택 후 나오는 대사
        { "DecideDeckCard_Str", new DialogData("???", "좋은 선택인걸? 네 덱이 한층 강해졌어!", default) },
        { "DecideDeckCard_Mind", new DialogData("???", "멋진 카드야! 네 덱이 더 견고해졌어.", default) },
        { "DecideDeckCard_Luck", new DialogData("???", "훌륭한 카드다!", default) },
        { "DecideDeckCard_Common", new DialogData("???", "오호라.. 아주 기대되는 선택이야.", default) },
    };
    public static Dictionary<string, DialogData> Dialogs = new Dictionary<string, DialogData>()
    {
        // 튜토리얼 : 인트로
        { "Tutorial_Intro_1", new DialogData("???", "반가워. 너도 나와 카드 게임 한 판 할까? 페이트잭 말이야!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_2", new DialogData("???", "그냥 간단한 놀이는 아닐거야. 어쩌면 도전? 어쩌면 진실..?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_3", new DialogData("???", "긴 말은 됐고 규칙부터 알려줄게!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_4", new DialogData("???", "페이트잭은 카드를 내서 <Accent_Basic>힘<sprite name=Str></Accent_Basic>과 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>, 그리고 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 올리고, 올린 능력치로 피해를 주어 상대방의 <Accent_Basic>체력<sprite name=Health></Accent_Basic>을 먼저 0으로 만드는 게임이야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_5", new DialogData("???", "너는 <Accent_Basic>체력<sprite name=Health></Accent_Basic>가 0이 되면 그 즉시 사망! 목숨이 단 하나지!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_6", new DialogData("???", "나는 0이 되어도 되살아나. 왜냐고? 단순해! 내 목숨은 셀 수가 없거든!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_7", new DialogData("???", "그래도 안 죽는 상대방과 게임을 하는건 재미없잖아? 너가 21번의 시련을 모두 이겨내면 내가 죽은척이라도 해줄게!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_8", new DialogData("???", "그래도 불리하다고 생각하지 너!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_9", new DialogData("???", "대신 나는 카드를 많이 내지 않아. 머리쓰는건 싫거든!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_10", new DialogData("???", "이 정도면 공평하지?", S_ActivateBtnEnum.Btn_Next) },

        // 튜토리얼 : 카드 내기
        { "Tutorial_Hit_1", new DialogData("???", "시련을 시작하기 전에 나는 내 <Accent_Basic>체력</Accent_Basic>과 내가 이번 시련에서 사용할 카드를 보여줄거야.", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "Tutorial_Hit_2", new DialogData("???", "나는 시련 동안 카드를 내거나 하지는 않아. 항상 고정되어있고 카드는 너가 열심히 내야해.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_3", new DialogData("???", "흠흠.. 그럼 본격적으로 페이트잭에 대해 알려줄게.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_4", new DialogData("???", "먼저 나와 다르게 넌 카드를 내야겠지?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_5", new DialogData("???", "카드를 낼 땐 2장의 보기 중 하나를 선택할 수 있어.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_6", new DialogData("???", "그럼 카드 내기를 눌러서 카드를 내봐.", S_ActivateBtnEnum.Btn_Hit) },

        // 튜토리얼 : 카드에 대해서
        { "Tutorial_Card_1", new DialogData("???", "잘했어! 그렇게 카드를 내는거야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_2", new DialogData("???", "카드 왼쪽 상단에 있는 숫자가 무게야. 하단에 게이지 보이지? 카드의 무게만큼 저 게이지가 채워져.", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "Tutorial_Card_3", new DialogData("???", "그리고 무게가 네 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>를 넘기 전까지만 카드를 낼 수 있어.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_4", new DialogData("???", "만약 카드를 냈는데 무게가 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>을 초과한다? 그러면 <Accent_Basic>버스트</Accent_Basic>가 돼. <Accent_Basic>버스트</Accent_Basic>엔 너가 주는 피해량이 0.5배가 되고 너가 받는 피해량은 2배 증가해.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_5", new DialogData("???", "무게가 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>와 같다면 <Accent_Basic>완벽</Accent_Basic>이 돼. <Accent_Basic>완벽</Accent_Basic> 시에는 너가 주는 피해량이 무조건 2배가 돼.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_6", new DialogData("???", "그러니 카드를 낼 때 신중해야겠지?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_7", new DialogData("???", "그리고 너가 카드를 냈던 곳이 <Accent_Basic>필드</Accent_Basic>이고 아직 내지 않은 카드가 있는 곳이 <Accent_Basic>덱</Accent_Basic>이야.", S_ActivateBtnEnum.Obj_FieldCards) },
        { "Tutorial_Card_8", new DialogData("???", "음.. 그리고 또.. 카드는 <Accent_Basic>지속</Accent_Basic> 효과와 <Accent_Basic>발현</Accent_Basic> 효과로 나뉘어져있어.", S_ActivateBtnEnum.Obj_FieldCards) },
        { "Tutorial_Card_9", new DialogData("???", "<Accent_Basic>지속</Accent_Basic> 효과는 <Accent_Basic>필드</Accent_Basic>에 있는 동안 계속 적용되고 <Accent_Basic>발현</Accent_Basic> 효과는 스탠드를 했을 때 발동해.", S_ActivateBtnEnum.Obj_FieldCards) },
        { "Tutorial_Card_10", new DialogData("???", "또한 <Accent_Basic>발현</Accent_Basic> 효과는 순서대로 발동해! 그러니 내는 순서도 중요하겠지?", S_ActivateBtnEnum.Obj_FieldCards) },
        { "Tutorial_Card_11", new DialogData("???", "그리고 네 모든 카드가 발동을 마치고 나서 내 카드가 발동해.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_12", new DialogData("???", "그럼 먼저 왼쪽 하단에 카드 더미를 눌러서 네 <Accent_Basic>덱</Accent_Basic>을 보고와바! 카드 더미를 다시 누르면 다시 <Accent_Basic>필드</Accent_Basic>로 올거야.", S_ActivateBtnEnum.Obj_ViewDeck) },

        // 튜토리얼 : 스탠드
        { "Tutorial_Stand_1", new DialogData("???", "어때 네 <Accent_Basic>덱</Accent_Basic>? 내가 아주 기본적인 카드만 먼저 채워놨어.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Stand_2", new DialogData("???", "이제 그럼 스탠드를 눌러봐.", S_ActivateBtnEnum.Btn_Stand) },

        // 튜토리얼 : 아웃트로
        { "Tutorial_Outro_1", new DialogData("???", "잘했어. <Accent_Basic>필드</Accent_Basic>에 있는 카드는 스탠드 후에 <Accent_Basic>사용한 카드 더미</Accent_Basic>로 이동하게 돼.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_2", new DialogData("???", "<Accent_Basic>덱</Accent_Basic>에 있는 카드를 다 쓰면 <Accent_Basic>사용한 카드 더미</Accent_Basic>의 카드로 <Accent_Basic>덱</Accent_Basic>을 채울거야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_3", new DialogData("???", "음.. 거의 다 말해준 것 같네!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_4", new DialogData("???", "그리고 나를 이길 때마다 아주 훌륭한 보상이 기다릴거야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_5", new DialogData("???", "보상에는 카드도 있지만 <Accent_Basic>각인</Accent_Basic>도 있어!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_6", new DialogData("???", "카드에 <Accent_Basic>각인</Accent_Basic>을 새겨서 효과를 추가하는거야. 너만의 강력한 카드가 탄생하는거지!!! 멋있겠다!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_7", new DialogData("???", "그럼 설명은 여기까지! 운명을 건 페이트잭을 시작해볼까?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Outro_8", new DialogData("???", "오랫동안 버텨야돼! 이렇게 열심히 설명해줬는데 말이야!!!", S_ActivateBtnEnum.Btn_Next) },

        // 튜토리얼 : 보상
        { "Tutorial_Reward_1", new DialogData("???", "시작하기에 앞서 카드를 좀 채우게 해줄게.", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "Tutorial_Reward_2", new DialogData("???", "원하는 만큼 골라봐!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },

        // 대사
        // 0
        { "FoeDialog_Reward_0_1", new DialogData("???", "어서와! 그럼 페이트잭을 시작해볼까?", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 1
        { "FoeDialog_EndTrial_1_1", new DialogData("???", "제법인걸?", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_EndTrial_1_2", new DialogData("???", "이라고 할 줄 알았지!!!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_EndTrial_1_3", new DialogData("???", "처음이니까 나도 설렁설렁한거야~!~!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },

        { "FoeDialog_Reward_1_1", new DialogData("???", "덱에 추가할 카드를 고르는 것도 좋지만!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_Reward_1_2", new DialogData("???", "다음 시련의 체력과 카드를 확인하는 것도 잊지마!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 2
        { "FoeDialog_EndTrial_2_1", new DialogData("???", "잘했어! 이번에도 이겼네!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 3
        { "FoeDialog_EndTrial_3_1", new DialogData("???", "잘하네!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },

        { "FoeDialog_Reward_3_1", new DialogData("???", "다음 시련부터는 내가 내는 카드가 1장 추가될거야.", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_Reward_3_2", new DialogData("???", "즉 어려워진다는거지!!! 너가 바로 실패할 수도 있으니까 이번엔 보상을 좀 더 후하게 줄게!!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) }, 
        // 4
        { "FoeDialog_EndTrial_4_1", new DialogData("???", "내 카드가 늘었지만 잘 이겨냈네!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 5
        { "FoeDialog_EndTrial_5_1", new DialogData("???", "잘했어!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 6
        { "FoeDialog_EndTrial_6_1", new DialogData("???", "잘했어! 벌써 6번째 시련까지 이겨냈어!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },

        { "FoeDialog_Reward_6_1", new DialogData("???", "다음 시련부터는 심판 카드도 등장할거야.", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_Reward_6_2", new DialogData("???", "심판은 한계와 무게 차이만큼 체력을 잃는 카드야. 좀 더 신경쓸 게 많아지겠지?", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_Reward_6_3", new DialogData("???", "대신에 각인 보상을 줄게! 잘해봐!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 7
        { "FoeDialog_EndTrial_7_1", new DialogData("???", "심판 카드도 잘 간파했어! 멋있는데!!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 8
        { "FoeDialog_EndTrial_8_1", new DialogData("???", "잘했어!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        // 9
        { "FoeDialog_EndTrial_9_1", new DialogData("???", "잘했어! 벌써 9번째 시련까지 이겨냈어!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },

        { "FoeDialog_Reward_9_1", new DialogData("???", "다음 시련부터는 카드가 1장 더 추가될거야!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_Reward_9_2", new DialogData("???", "어렵겠지만 잘 할 수 있을거야! 여태 잘해왔으니까!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
    };

    public static DialogData GetMonologs(string key)
    {
        if (Monologs.ContainsKey(key))
        {
            return Monologs[key];
        }

        return default;
    }

    public static List<DialogData> GetDialogsByPrefix(string prefix)
    {
        return Dialogs
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
