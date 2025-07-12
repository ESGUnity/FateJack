using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class S_TextHelper
{
    static readonly Dictionary<string, string> AccentColorHexMap = new()
    {
        { "Accent_Curse", "8C47BF" },
        { "Accent_Basic", "DBC46B" },
        { "Accent_Engraving", "7AD1D6" },
    };

    public static string WrapText(string input, int maxLen = 15)
    {
        StringBuilder result = new();
        int visibleCount = 0;
        bool insideTag = false;

        int lastSpaceIndexInResult = -1;      // 마지막 공백 위치 (result 인덱스)
        int visibleCountAtLastSpace = 0;      // 그 시점의 visibleCount

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '<')
                insideTag = true;

            result.Append(c);

            if (c == '>')
                insideTag = false;

            if (insideTag)
                continue; // 태그 안 글자는 visibleCount에 포함 안 함

            // 공백 문자
            if (c == ' ')
            {
                lastSpaceIndexInResult = result.Length - 1; // 공백 위치 저장
                visibleCountAtLastSpace = visibleCount;
            }

            if (c != '\n' && c != ' ')
            {
                visibleCount++;
            }

            // visibleCount가 maxLen 이상일 때 줄바꿈 처리
            if (visibleCount >= maxLen)
            {
                if (lastSpaceIndexInResult >= 0)
                {
                    // 마지막 공백을 줄바꿈으로 바꾸고 visibleCount 조정
                    result[lastSpaceIndexInResult] = '\n';
                    visibleCount = visibleCount - visibleCountAtLastSpace;
                    lastSpaceIndexInResult = -1;
                }
                else
                {
                    // 공백이 없으면 바로 줄바꿈 추가 (단어 중간에 끊는 것)
                    result.Append('\n');
                    visibleCount = 0;
                }
            }

            if (c == '\n')
            {
                visibleCount = 0;
                lastSpaceIndexInResult = -1;
            }
        }

        return result.ToString();
    }
    public static string ParseText(string text, S_CardBase card = null)
    {
        // 0. 제일 앞에 <Accent_ExpectedHarmValue> 태그가 있을 경우 처리
        if (text.StartsWith("<Accent_ExpectedHarmValue>"))
        {
            // 태그 제거
            int endTagIndex = text.IndexOf("</Accent_ExpectedHarmValue>");
            if (endTagIndex != -1)
            {
                text = text.Substring(endTagIndex + "</Accent_ExpectedHarmValue>".Length).TrimStart();

                if (card != null)
                {
                    text += $"\n(예상 피해량 : {card.ExpectedValue})";
                }
            }
        }

        // 1. 기본 색상 태그 처리 (특수 태그 제외)
        foreach (var kvp in AccentColorHexMap)
        {
            string tagName = kvp.Key;
            string colorHex = kvp.Value;

            string pattern = $@"<{tagName}>(.*?)</{tagName}>";
            string replacement = $"<b><color=#{colorHex}>$1</color></b>";

            text = Regex.Replace(text, pattern, replacement, RegexOptions.Singleline);
        }
        // 2. 특수 태그: Accent_MultiPerHitCount
        string multiHitPattern = @"<Accent_MultiPerHitCount>\d+</Accent_MultiPerHitCount>";
        text = Regex.Replace(text, multiHitPattern, match =>
        {
            if (card == null || card.Unleash == null || card.Persist == null) return match.Value; // 카드 정보 없으면 원문 유지

            try
            {
                int unleash = card.Unleash.First().Value; // First가 가능한 이유는 이런 효과는 무조건 1개이기 때문.
                int persist = card.Persist.First().Value;
                int hit = card.HitCount;

                if (hit <= 0) return match.Value;

                int finalValue = unleash * persist * hit;
                return $"<b><color=#57B842>{finalValue}</color></b>";
            }
            catch
            {
                return match.Value;
            }
        });
        // 3. 특수 태그: Accent_AddPerRebound
        string addReboundPattern = @"<Accent_AddPerRebound>\d+</Accent_AddPerRebound>";
        text = Regex.Replace(text, addReboundPattern, match =>
        {
            if (card == null || card.Unleash == null || card.Persist == null)
                return match.Value;

            try
            {
                int unleash = card.Unleash.First().Value;
                int persist = card.Persist.First().Value;
                int rebound = card.ReboundCount;

                int result = unleash + (persist * rebound);
                return $"<b><color=#57B842>{result}</color></b>";
            }
            catch
            {
                return match.Value;
            }
        });

        return text;
    }
}
