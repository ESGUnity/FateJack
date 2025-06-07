using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class S_DialogMetaData
{
    static Dictionary<string, string> monologs = new Dictionary<string, string>()
    {
        // 데달로스
        { "Daedalus_Intro", "상점에 오신 걸 환영합니다!\n마음껏 둘러보세요." },
        { "Daedalus_StoneOfInsight", "그건 깨달음의 돌입니다.\n무작위 능력 3개 중 1개를 얻을 수 있습니다." },
        { "Daedalus_GrandBlueprint", "그건 위대한 설계도입니다.\n덱에서 카드를 1장 선택하여 문양과 숫자, 그리고 모든 요소를 재설정합니다." },
        { "Daedalus_DarkRedBrush", "그건 검붉은 붓입니다.\n덱에서 카드를 1장 선택하여 문양을 재설정합니다." },
        { "Daedalus_DiceOfEris", "그건 에리스의 주사위입니다.\n덱에서 카드를 1장 선택하여 숫자를 재설정합니다." },
        { "Daedalus_AstroTool", "그건 점성술 도구입니다.\n덱에서 카드를 1장 선택하여 조건, 조건 제약, 디버프 제약을 재설정합니다." },
        { "Daedalus_FingerOfMomus", "그건 Daedalus_FingerOfMomus입니다.\n덱에서 카드를 1장 선택하여 효과, 추가 효과를 재설정합니다." },
        { "Daedalus_OldLoom", "그건 낡은 직조기입니다.\n덱에서 카드를 1장 선택하여 조건 제약을 제외한 모든 요소를 재설정합니다." },
        { "Daedalus_SacredSeal", "그건 신성한 인장입니다.\n덱에서 카드를 1장 선택하여 조건을 제외한 모든 요소를 재설정합니다." },
        { "Daedalus_CurseOfMoros", "조심하세요! 그건 모로스의 저주입니다.\n덱에서 카드를 1장 선택하여 디버프 제약을 부여하고 모든 요소를 재설정합니다." },
        { "Daedalus_EmberOfPluto", "그건 플루토의 잔불입니다.\n덱에서 카드를 1장 선택하여 디버프 제약을 없애고 모든 요소를 재설정합니다." },
        { "Daedalus_OldNeedle", "그건 낡은 바늘입니다.\n덱에서 카드를 1장 선택하여 추가 효과를 제외한 모든 요소를 재설정합니다." },
        { "Daedalus_OldScissors", "그건 낡은 가위입니다.\n덱에서 카드를 1장 선택하여 효과를 제외한 모든 요소를 재설정합니다." },
        { "Daedalus_OracleBall", "이건 다음 적의 체력과 능력을 보여주는 예지 구슬이에요.\n" },
        { "Daedalus_ParallizeLight", "그건 신경마비 광원입니다.\n당신 능력 중 1개를 제거합니다." },
        { "Daedalus_PostureCorrector", "그건 자세 교정기입니다.\n당신 능력 중 1개를 선택하여\n보유 능력 중 맨 앞으로 위치하게 만듭니다.\n능력 발동 순서가 중요한 순간도 있죠." },
    };
    static Dictionary<string, DialogData> dialogDatas = new Dictionary<string, DialogData>()
    {
        // 튜토리얼 인트로
        { "Tutorial_Intro_1", new DialogData("???", "반가워. 신들의 공간에 온 걸 환영해.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_2", new DialogData("???", "이곳은 인간들의 운명을 지키는 공간.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_3", new DialogData("???", "간혹 너처럼 의지가 강한 인간은 정해진 운명을 거부해.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_4", new DialogData("???", "운명에 저항한 인간은 이곳에 오게 되지.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_5", new DialogData("???", "여기서 네 의지를 증명한다면 정해진 운명을\n네가 직접 쓸 수 있는 기회가 주어지지.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_6", new DialogData("???", "앞으로 운명을 수호하는 강력한 존재들이 널 막을거야.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_7", new DialogData("???", "그럼 어떻게 그런 존재들을 상대할 수 있을까?", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_8", new DialogData("???", "그건 바로 카드야. 카드는 곧 네 운명이지.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_9", new DialogData("???", "예전엔 실로 운명을 만들었지만 지금은 카드로 운명을 만든다고 해.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_10", new DialogData("???", "네 운명인 카드를 통해 운명을 수호하는 존재를 이길 수 있어.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Intro_11", new DialogData("???", "히트 버튼을 눌러서 카드를 내봐.", S_ActivateUIEnum.HitBtn) },

         // 튜토리얼 히트, 의지 히트, 비틀기, 스탠드
        { "Tutorial_Action_1", new DialogData("???", "잘했어. 그렇게 카드를 내는 것을 히트라고 해.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Action_2", new DialogData("???", "그리고 의지를 1 소모하여 의지 히트를 할 수 있고 원하는 카드를 직접 낼 수 있어.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Action_3", new DialogData("???", "스탠드 버튼을 누르면 다시 숫자 합이 0으로 돌아가고 턴이 마무리 돼.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Action_4", new DialogData("???", "하지만 너가 상대하는 존재의 공격을 받고 체력이 1 감소하게 돼.\n체력이 0이 되면 네 운명은 정해진대로 끝나겠지.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Action_5", new DialogData("???", "비틀기 버튼은 한 턴에 낸 카드를 모두 되돌릴 수 있어.\n비틀기도 의지를 1 소모해.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Action_6", new DialogData("???", "단 모든 능력치가 카드를 내기 전으로 돌아가고 낸 카드도 제외돼.\n제외된 카드는 해당 시련에서는 더 사용할 수 없어.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Action_7", new DialogData("???", "비틀기 버튼을 눌러 카드를 낸 적 없는 상태로 돌려봐.\n시련을 거듭할수록 쓸모가 있을거야.", S_ActivateUIEnum.TwistBtn) },

        // 튜토리얼 카드 설명
        { "Tutorial_Card_1", new DialogData("???", "이어서 카드와 능력에 대해 설명해줄게.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Card_2", new DialogData("???", "카드를 내면 숫자만큼 네 숫자 합이 올라.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_3", new DialogData("???", "그리고 숫자 합에는 최대치인 한계가 있어.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_4", new DialogData("???", "숫자 합이 한계보다 작은 것은 문제가 안돼.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_5", new DialogData("???", "그리고 숫자 합이 한계와 같으면 클린히트라는 버프를 얻고 너가 주는 모든 데미지가 1.5배 상승해.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_6", new DialogData("???", "그러나 한계를 넘기게 되면 오직 스탠드만 할 수 있게 되고 모든 데미지가 절반 감소하게 돼.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_7", new DialogData("???", "그러니 한계를 넘기지 않게 조심하는 것이 좋겠지.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_8", new DialogData("???", "또한 카드의 숫자는 문양에 따라 힘, 정신력, 행운 능력치를 올릴 수 있어.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_9", new DialogData("???", "카드에는 스페이드, 하트, 다이아몬드, 클로버까지 4개의 문양이 존재해.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_10", new DialogData("???", "문양이 스페이드라면 힘, 하트는 정신력, 다이아몬드는 행운,\n클로버는 무작위 능력치 하나를 숫자만큼 올려줘.", S_ActivateUIEnum.StatInfoCanvas) },
        { "Tutorial_Card_11", new DialogData("???", "그리고 카드에는 특수한 효과와 효과가 발동하기 위한 조건이 있어.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Card_12", new DialogData("???", "상세하게는 조건에는 조건과 조건 제약, 디버프 제약이 있고\n효과에는 효과와 추가 효과가 있어.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Card_13", new DialogData("???", "시련을 진행하다보면 자연스럽게 알게 될 거야.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Card_14", new DialogData("???", "마지막으로 능력에 대해 설명해줄게. 스탠드 버튼을 눌러봐.", S_ActivateUIEnum.StandBtn) },

        // 튜토리얼 능력
        { "Tutorial_Skill_1", new DialogData("???", "오른쪽 밑에 있는게 네 능력이야.", S_ActivateUIEnum.skillInfoCanvas) },
        { "Tutorial_Skill_2", new DialogData("???", "능력도 카드처럼 조건과 효과가 있어.", S_ActivateUIEnum.skillInfoCanvas) },
        { "Tutorial_Skill_3", new DialogData("???", "방금 발동한 도륙은 스탠드 시 무작위 능력치 2개를 곱한만큼 피해를 주지.", S_ActivateUIEnum.skillInfoCanvas) },
        { "Tutorial_Skill_4", new DialogData("???", "그렇게 상대의 체력을 먼저 0으로 만들면 시련을 극복할 수 있어.", S_ActivateUIEnum.skillInfoCanvas) },
        { "Tutorial_Skill_5", new DialogData("???", "그 외에도 능력치를 올려주거나 더욱 특수한 효과를 지닌 능력도 많아.", S_ActivateUIEnum.skillInfoCanvas) },
        { "Tutorial_Skill_6", new DialogData("???", " 네 카드에 어울리는 능력을 지닌다면 더욱 시너지를 낼 수도 있겠지.", S_ActivateUIEnum.skillInfoCanvas) },
        { "Tutorial_Skill_7", new DialogData("???", "설명은 여기까지야. 네 의지로 운명을 거슬러봐.", S_ActivateUIEnum.NextBtn) },
        { "Tutorial_Skill_8", new DialogData("???", "힘내. 또 보자고.", S_ActivateUIEnum.NextBtn) },
    };

    public static string GetMonologData(string key)
    {
        if (monologs.ContainsKey(key))
        {
            return monologs[key];
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
