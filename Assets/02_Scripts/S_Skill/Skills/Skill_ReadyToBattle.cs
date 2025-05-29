using System.Threading.Tasks;

public class Skill_ReadyToBattle : S_Skill
{
    public Skill_ReadyToBattle() : base
    (
        "Skill_ReadyToBattle",
        "���� �غ�",
        "�÷� ���� �� ���ڰ� 0�� ������ ī�� 4���� �����Ͽ� ��Ʈ�մϴ�.",
        S_SkillConditionEnum.StartTrial,
        S_SkillPassiveEnum.None,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = false;
        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        for (int i = 0; i < 4; i++)
        {
            await eA.CreationCard(this, null, 0, S_CardSuitEnum.Random);
        }
    }
    public override S_Skill Clone()
    {
        return new Skill_ReadyToBattle();
    }
}
