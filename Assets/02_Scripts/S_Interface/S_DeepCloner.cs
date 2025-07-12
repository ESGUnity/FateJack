using Newtonsoft.Json;
using System.Linq;

public static class S_DeepCloner
{
    public static T DeepClone<T>(T obj)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        var json = JsonConvert.SerializeObject(obj, settings);
        var clone = JsonConvert.DeserializeObject<T>(json, settings);

        if (clone is S_CardBase card)
        {
            // 중복 제거 함수 직접 작성
            card.Unleash = card.Unleash?.Distinct().ToList();
            card.Persist = card.Persist?.Distinct().ToList();
            card.OriginEngraving = card.OriginEngraving?.Distinct().ToList();
            card.Engraving = card.Engraving?.Distinct().ToList();
        }

        return clone;
    }
}