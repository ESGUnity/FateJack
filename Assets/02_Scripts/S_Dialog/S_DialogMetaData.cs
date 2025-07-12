using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class S_DialogMetaData
{
    public static Dictionary<string, DialogData> Monologs = new Dictionary<string, DialogData>()
    {
        // 히트 관련
        { "Hit_BurstOrPerfect", new DialogData("???", "버스트나 완벽 시엔 카드를 낼 수 없다고!!!", default) },
        { "Hit_NoCardInDeckAndUsed", new DialogData("???", "더 이상 낼 카드가 없는데?", default) },

        // 보상
        { "Reward_Start", new DialogData("???", "자 맘껏 골라봐!", default) },

        // 구매 전 포인터 올려놨을 때 대사
        { "Reward_OldMold", new DialogData("???", "그건 낡은 거푸집이야! 무작위 카드 5장 중에서 1장을 골라!", default) },
        { "Reward_MeltedMold", new DialogData("???", "그건 녹아내린 거푸집이야! 엄청 뜨겁네!! 무작위 힘 카드 3장 중에서 1장을 고르게 해줄게!", default) },
        { "Reward_SpiritualMold", new DialogData("???", "오오.. 그건 영험한 거푸집! 무작위 정신력 카드 3장 중에서 1장을 고르게 해줄게!", default) },
        { "Reward_BrightMold", new DialogData("???", "으앗 눈부셔!! 빛나는 거푸집이다! 무작위 행운 카드 3장 중에서 1장을 고르게 해줄게!!", default) },
        { "Reward_DelicateMold", new DialogData("???", "그건 정교한 거푸집이야. 무작위 공용 카드 3장 중에서 1장을 고르게 해줄게!", default) },

        // 구매 후 무엇을 고르는건지 알려주는 대사
        { "Reward_OldMold_Buied", new DialogData("???", "딱 1장만 고를 수 있어! 과연 무엇을 고를까?", default) },
        { "Reward_MeltedMold_Buied", new DialogData("???", "딱 1장만 고를 수 있어! 빨리 골라봐!!!", default) },
        { "Reward_SpiritualMold_Buied", new DialogData("???", "딱 1장만 고를 수 있어! 과연 무엇을 고를까?", default) },
        { "Reward_BrightMold_Buied", new DialogData("???", "딱 1장만 고를 수 있어! 과연 무엇을 고를까?", default) },
        { "Reward_DelicateMold_Buied", new DialogData("???", "딱 1장만 고를 수 있어! 과연 무엇을 고를까?", default) },
    };
    public static Dictionary<string, DialogData> Dialogs = new Dictionary<string, DialogData>()
    {
        // 튜토리얼 : 인트로
        { "Tutorial_Intro_1", new DialogData("???", "반가워. 너도 나와 카드 게임 한 판 할까? 페이트잭 말이야!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_2", new DialogData("???", "그냥 간단한 놀이는 아닐거야. 어쩌면 도전? 어쩌면 벌? 아니면.. 진실?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_3", new DialogData("???", "긴 말은 됐고 규칙부터 알려줄게!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_4", new DialogData("???", "페이트잭은 카드를 내서 <Accent_Basic>힘<sprite name=Str></Accent_Basic>, <Accent_Basic>Mind</Accent_Basic>, <Accent_Basic>Luck</Accent_Basic>을 올리고, 올린 능력치로 피해를 주어 상대방의 <Accent_Basic>Health</Accent_Basic>를 먼저 0으로 만드는 게임이야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_5", new DialogData("???", "너는 <Accent_Basic>Health</Accent_Basic>가 0이 되면 그 즉시 사망! 목숨이 단 하나지!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_6", new DialogData("???", "나는 0이 되어도 되살아나. 왜냐고? 단순해! 내 목숨은 셀 수가 없거든!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_7", new DialogData("???", "그래도 안 죽는 상대방과 게임하는건 재미없잖아? 21번만 나를 이기면 내가 죽은척해줄게!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_8", new DialogData("???", "그래도 불리하다고 생각하지 너!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_9", new DialogData("???", "대신 나는 카드를 많이 내지 않아. 머리쓰는건 싫거든!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Intro_10", new DialogData("???", "이 정도면 공평하지?", S_ActivateBtnEnum.Btn_Next) },

        // 튜토리얼 : 카드 내기
        { "Tutorial_Hit_1", new DialogData("???", "그럼 본격적으로 알려줄게.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_2", new DialogData("???", "먼저 카드를 내야겠지?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_3", new DialogData("???", "카드를 낼 땐 2장의 보기 중 하나를 선택할 수 있어.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Hit_4", new DialogData("???", "그럼 카드 내기를 눌러서 카드를 내봐.", S_ActivateBtnEnum.Btn_Hit) },

        // 튜토리얼 : 카드에 대해서
        { "Tutorial_Card_1", new DialogData("???", "잘했어! 그렇게 카드를 내는거야.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_2", new DialogData("???", "카드 왼쪽 상단에 있는 숫자가 무게야. 하단에 게이지 보이지? 카드의 무게만큼 저 게이지가 채워져.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_3", new DialogData("???", "그리고 네 <Accent_Basic>Limit</Accent_Basic>까지만 카드를 낼 수 있어.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_4", new DialogData("???", "만약 무게가 <Accent_Basic>Limit</Accent_Basic>을 초과한다? 그러면 <Accent_Basic>버스트</Accent_Basic>가 돼. <Accent_Basic>버스트</Accent_Basic>엔 너가 주는 피해량이 0.5배가 되고 너가 받는 피해량은 2배 증가해.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_5", new DialogData("???", "무게가 <Accent_Basic>Limit</Accent_Basic>와 같다면 <Accent_Basic>완벽</Accent_Basic>이 돼. <Accent_Basic>완벽</Accent_Basic> 시에는 너가 주는 피해량이 무조건 2배가 돼.", S_ActivateBtnEnum.UI_Stat) },
        { "Tutorial_Card_6", new DialogData("???", "그러니 카드를 낼 때 신중해야겠지?", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_7", new DialogData("???", "그리고 너가 카드를 냈던 곳이 <Accent_Basic>필드</Accent_Basic>이고 아직 내지 않은 카드가 있는 곳이 <Accent_Basic>덱</Accent_Basic>이야.", S_ActivateBtnEnum.Obj_FieldCards) },
        { "Tutorial_Card_8", new DialogData("???", "음.. 그리고 또.. 카드는 지속 효과와 발현 효과로 나뉘어져있어.", S_ActivateBtnEnum.Obj_FieldCards) },
        { "Tutorial_Card_9", new DialogData("???", "지속 효과는 <Accent_Basic>필드</Accent_Basic>에 있는 동안 계속 적용되고 발현 효과는 스탠드를 했을 때 발동해.", S_ActivateBtnEnum.Obj_FieldCards) },
        { "Tutorial_Card_10", new DialogData("???", "내 카드도 마찬가지야!", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Card_11", new DialogData("???", "그럼 먼저 왼쪽 하단에 카드 더미를 눌러서 네 <Accent_Basic>덱</Accent_Basic>을 보고와바! 카드 더미를 다시 누르면 다시 <Accent_Basic>필드</Accent_Basic>로 올거야.", S_ActivateBtnEnum.Obj_ViewDeck) },

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
        { "Tutorial_Reward_1", new DialogData("???", "시작하기에 앞서 카드를 좀 채우게 해줄게.", S_ActivateBtnEnum.Btn_Next) },
        { "Tutorial_Reward_2", new DialogData("???", "원하는 만큼 골라봐!", S_ActivateBtnEnum.Btn_Next) },

        // 대사
        { "FoeDialog_StartTrial_1_1", new DialogData("???", "시작하기에 앞서 카드를 좀 채우게 해줄게.", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_StartNewTurn_1_1", new DialogData("???", "시작하기에 앞서 카드를 좀 채우게 해줄게.", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },

        { "FoeDialog_EndTrial_1_1", new DialogData("???", "잘했어! 내가 졌네!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
        { "FoeDialog_EndTrial_1_2", new DialogData("???", "자 그럼 내 다음 체력과 카드를 보여줄게!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },

        { "FoeDialog_StartReward_1_1", new DialogData("???", "그리고 보상이야! 원하는 만큼 가져가!", S_ActivateBtnEnum.Btn_Next_NoBlackBackground) },
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
