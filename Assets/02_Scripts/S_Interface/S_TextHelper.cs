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

    public static string WrapText(string input, int maxLen)
    {
        StringBuilder result = new();
        int visibleCount = 0;
        bool insideTag = false;

        int lastSpaceIndexInResult = -1;
        int visibleCountAtLastSpace = 0;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '<')
                insideTag = true;

            result.Append(c);

            if (c == '>')
                insideTag = false;

            if (insideTag)
                continue;

            if (c == ' ')
            {
                lastSpaceIndexInResult = result.Length - 1;
                visibleCountAtLastSpace = visibleCount;
                continue; // 공백은 visibleCount에 포함하지 않음
            }

            if (c == '\n')
            {
                visibleCount = 0;
                lastSpaceIndexInResult = -1;
                continue;
            }

            visibleCount++;

            if (visibleCount >= maxLen)
            {
                if (lastSpaceIndexInResult >= 0)
                {
                    result[lastSpaceIndexInResult] = '\n';
                    visibleCount = visibleCount - visibleCountAtLastSpace;
                    lastSpaceIndexInResult = -1;
                }
                else
                {
                    result.Append('\n');
                    visibleCount = 0;
                }
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
        string multiHitPattern = @"<Accent_MultiPerHitCount>(\d+)</Accent_MultiPerHitCount>";
        text = Regex.Replace(text, multiHitPattern, match =>
        {
            string rawValue = match.Groups[1].Value; // 태그 안의 숫자

            if (card == null || card.Unleash == null || card.Persist == null) return rawValue; // 태그 제거, 숫자만 반환

            try
            {
                int unleash = card.Unleash.First().Value;
                int persist = card.Persist.First().Value;
                int hit = card.HitCount;

                if (hit <= 0) return rawValue;

                int finalValue = unleash * persist * hit;
                return $"<b><color=#57B842>{finalValue}</color></b>";
            }
            catch
            {
                return rawValue; // 에러 발생 시도 숫자만 반환
            }
        });
        // 3. 특수 태그: Accent_AddPerRebound
        string addReboundPattern = @"<Accent_AddPerRebound>(\d+)</Accent_AddPerRebound>";
        text = Regex.Replace(text, addReboundPattern, match =>
        {
            string rawValue = match.Groups[1].Value; // 태그 내부 숫자만 추출

            if (card == null || card.Unleash == null || card.Persist == null) return rawValue; // 태그 제거하고 숫자만 반환

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
                return rawValue; // 예외 발생 시 숫자만 반환
            }
        });

        return text;
    }
}
