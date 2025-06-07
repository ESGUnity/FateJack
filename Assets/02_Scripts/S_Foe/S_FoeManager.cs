using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class S_FoeManager : MonoBehaviour
{
    [Header("이번 게임의 모든 적")]
    Queue<(S_Foe, int)> allFoeQueue = new();

    [Header("적 능력치 관련")]
    const float BASIC_HEALTH_VALUE = 100;
    const float HEALTH_GROWTH_RATE = 1.4f;
    const float ELITE_GROWTH_RATE = 1.25f;
    const float BOSS_GROWTH_RATE = 1.7f;

    // 싱글턴
    static S_FoeManager instance;
    public static S_FoeManager Instance { get { return instance; } }

    void Awake()
    {
        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateFoeByStartGame()
    {
        GenerateFoesByType(S_FoeTypeEnum.Clotho, S_FoeTypeEnum.Clotho_Elite, S_FoeTypeEnum.Clotho_Boss);
        GenerateFoesByType(S_FoeTypeEnum.Lachesis, S_FoeTypeEnum.Lachesis_Elite, S_FoeTypeEnum.Lachesis_Boss);
        GenerateFoesByType(S_FoeTypeEnum.Atropos, S_FoeTypeEnum.Atropos_Elite, S_FoeTypeEnum.Atropos_Boss);
    }
    private void GenerateFoesByType(S_FoeTypeEnum normalType, S_FoeTypeEnum eliteType, S_FoeTypeEnum bossType)
    {
        List<int> eliteIndexes = GetEliteIndexes();

        for (int i = 0; i < 9; i++)
        {
            S_Foe foeInfo;
            int health;

            if (i == 8) // 마지막은 항상 보스
            {
                foeInfo = S_FoeList.GetRandomFoe(bossType);
                health = Mathf.RoundToInt(BASIC_HEALTH_VALUE * Mathf.Pow(HEALTH_GROWTH_RATE, i + 1) * ELITE_GROWTH_RATE);
            }
            else if (eliteIndexes.Contains(i))
            {
                foeInfo = S_FoeList.GetRandomFoe(eliteType);
                health = Mathf.RoundToInt(BASIC_HEALTH_VALUE * Mathf.Pow(HEALTH_GROWTH_RATE, i + 1) * ELITE_GROWTH_RATE);
            }
            else
            {
                foeInfo = S_FoeList.GetRandomFoe(normalType);
                health = Mathf.RoundToInt(BASIC_HEALTH_VALUE * Mathf.Pow(HEALTH_GROWTH_RATE, i + 1));
            }

            allFoeQueue.Enqueue((foeInfo, health));
        }
    }
    private List<int> GetEliteIndexes()
    {
        List<int> validIndexes = new List<int> { 2, 3, 4, 5, 6, 7 };
        List<int> result = new List<int>();

        while (true)
        {
            int first = validIndexes[Random.Range(0, validIndexes.Count)];
            List<int> remaining = validIndexes.Where(x => Mathf.Abs(x - first) > 1).ToList();

            if (remaining.Count == 0) continue;

            int second = remaining[Random.Range(0, remaining.Count)];

            result.Add(Mathf.Min(first, second));
            result.Add(Mathf.Max(first, second));
            break;
        }

        return result;
    }
    public void SpawnFoe()
    {
        (S_Foe, int) info = allFoeQueue.Dequeue();

        // 피조물 오브젝트 생성
        S_FoeInfo foeInfo = new S_FoeInfo();
        foeInfo.SetFoeInfoInfo(info.Item1, info.Item2);

        // UI 세팅
        S_FoeInfoSystem.Instance.SetFoe(foeInfo);
    }
    public (S_Foe, int) PeekNextFoe()
    {
        return allFoeQueue.Peek();
    }
}

public enum S_FoeAbilityConditionEnum
{
    None,
    StartTrial,
    Stand,
    Reverb,
    DeathAttack,
}
public enum S_FoeTypeEnum
{
    None,
    Clotho,
    Lachesis,
    Atropos,
    Clotho_Elite,
    Lachesis_Elite,
    Atropos_Elite,
    Clotho_Boss,
    Lachesis_Boss,
    Atropos_Boss,
}
public enum S_FoePassiveEnum
{
    None,
    NeedActivatedCount,
}