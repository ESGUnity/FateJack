using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class S_TutorialManager : MonoBehaviour
{
    // 싱글턴
    static S_TutorialManager instance;
    public static S_TutorialManager Instance { get { return instance; } }

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

    public async void StartTutorial()
    {
        await S_GameFlowManager.Instance.StartTrialAsync();

        await StartTutorialAsync();
    }
    async Task StartTutorialAsync()
    {
        Queue<DialogData> dialogs = new Queue<DialogData>();

        // 튜토리얼 인트로
        List<DialogData> dialogList = S_DialogMetaData.GetDialogsByPrefix("Tutorial_Intro");
        for (int i = 0; i < dialogList.Count; i++)
        {
            dialogs.Enqueue(dialogList[i]);
        }
        await S_DialogInfoSystem.Instance.StartDialogByInGame(dialogs);
        // 히트 시작
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;
        S_Card hitCard = S_PlayerCard.Instance.DrawRandomCard(1)[0];
        // 카드 내기
        await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(hitCard, S_CardOrderTypeEnum.BasicHit);
        // 우선이 있었다면 해제
        if (S_PlayerStat.Instance.IsFirst != S_FirstEffectEnum.None) await S_EffectActivator.Instance.AppliedFirstAsync();
        // 히트 카드 진행
        if (S_GameFlowManager.Instance.GetCardOrderQueueCount() <= 1)
        {
            await S_GameFlowManager.Instance.StartHittingCard();
        }

        // 튜토리얼 히트, 의지 히트, 비틀기, 스탠드
        dialogList = S_DialogMetaData.GetDialogsByPrefix("Tutorial_Action");
        for (int i = 0; i < dialogList.Count; i++)
        {
            dialogs.Enqueue(dialogList[i]);
        }
        await S_DialogInfoSystem.Instance.StartDialogByInGame(dialogs);
        // 비틀기
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;
        S_PlayerStat.Instance.UseDetermination();
        // 카드 제외 및 제외된 카드 복구. 이하 3개 메서드는 반드시 붙어다녀야한다.
        S_PlayerCard.Instance.ResetCardsByTwist(out List<S_Card> stacks, out List<S_Card> exclusions);
        await S_StackInfoSystem.Instance.ExclusionCardsByTwistAsync(stacks);
        await S_UICardEffecter.Instance.ReturnExclusionCardsByTwistAsync(exclusions);
        // 능력의 조건 체크
        S_PlayerSkill.Instance.CheckSkillMeetCondition();
        // 적의 조건 체크
        S_FoeInfoSystem.Instance.CheckFoeMeetCondition();
        // 스탯, 히스토리를 스택의 카드를 내기 전으로 되돌리기.
        S_PlayerStat.Instance.ResetStatsByTwist();
        // 비틀기로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.Twist);
        // 피조물이 죽었다면 전투 종료
        if (S_FoeInfoSystem.Instance.CurrentFoe.CurrentHealth <= 0)
        {
            S_GameFlowManager.Instance.EndTrial();
        }
        else // 아니라면 히트 다시 시작
        {
            S_GameFlowManager.Instance.StartNewTurn();
        }

        // 튜토리얼 카드
        dialogList = S_DialogMetaData.GetDialogsByPrefix("Tutorial_Card");
        for (int i = 0; i < dialogList.Count; i++)
        {
            dialogs.Enqueue(dialogList[i]);
        }
        await S_DialogInfoSystem.Instance.StartDialogByInGame(dialogs);
        // 스탠드
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;
        // 카드의 결의 효과 발동
        await S_EffectActivator.Instance.ActivatedResolveCard();
        // 능력 발동
        await S_PlayerSkill.Instance.ActivateStandSkillsByStand();
        // 적 발동
        await S_FoeInfoSystem.Instance.ActivateStandFoeByStand();
        // 카드오더큐가 1개라면, 즉 시련 시작 시 혹은 스탠드 시에 창조되었다면, 카드에 의해 창조된게 아니라면
        if (S_GameFlowManager.Instance.GetCardOrderQueueCount() >= 1)
        {
            await S_GameFlowManager.Instance.StartHittingCard();
        }
        // 카드 고정. IsCurrentHit을 false로 만드는 과정
        S_PlayerCard.Instance.FixCardsByStand();
        // 더 이상 데미지가 되돌아가지 않는다.
        S_FoeInfoSystem.Instance.FixHealthByStand();
        // 스탠드로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.Stand);
        // 피조물 처치 여부에 따른 처리
        if (S_FoeInfoSystem.Instance.CurrentFoe.OldHealth <= 0) // 피조물이 죽었다면 전투 종료
        {
            S_GameFlowManager.Instance.EndTrial();
        }
        else // 살았다면 피조물이 플레이어를 공격
        {
            // 플레이어 공격
            await S_FoeInfoSystem.Instance.AttackPlayer();

            // 공격받고 나서 스택 합을 0으로 만들고 클린히트, 버스트 초기화하자.
            S_PlayerStat.Instance.ResetStackSum();
            S_PlayerStat.Instance.CheckBurstAndCleanHit();
            S_StatInfoSystem.Instance.ChangeSpecialAbility();

            // 히트 다시 시작
            S_GameFlowManager.Instance.StartNewTurn();
        }

        // 튜토리얼 능력
        dialogList = S_DialogMetaData.GetDialogsByPrefix("Tutorial_Skill");
        for (int i = 0; i < dialogList.Count; i++)
        {
            dialogs.Enqueue(dialogList[i]);
        }
        await S_DialogInfoSystem.Instance.StartDialogByInGame(dialogs);

        // 튜토리얼 완료
        PlayerPrefs.SetInt("TutorialCompleted", 1); // 튜토리얼 완수했음을 알리는 로직
        PlayerPrefs.Save();
    }
}
