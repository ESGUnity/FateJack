using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class S_FoeManager : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject prefab_Foe;

    [Header("�̹� ������ ��� ��")]
    Queue<(S_Foe, int)> allFoeQueue = new();

    [Header("�� �ɷ�ġ ����")]
    const float BASIC_HEALTH_VALUE = 30;
    const float HEALTH_GROWTH_RATE = 1.4f;
    const float ELITE_GROWTH_RATE = 1.25f;
    const float BOSS_GROWTH_RATE = 1.7f;

    // �̱���
    static S_FoeManager instance;
    public static S_FoeManager Instance { get { return instance; } }

    void Awake()
    {
        // �̱���
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

            if (i == 8) // �������� �׻� ����
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

        // ������ ������Ʈ ����
        GameObject go = Instantiate(prefab_Foe);
        go.GetComponent<S_FoeObject>().SetCreatureInfo(info.Item1, info.Item2);

        // UI ����
        S_FoeInfoSystem.Instance.SetFoe(go.GetComponent<S_FoeObject>());
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