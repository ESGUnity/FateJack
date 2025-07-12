using DG.Tweening;
using UnityEngine;

public class S_FieldCardObj : S_CardObj
{
    const float LEAN_VALUE = 3.5f;
    [HideInInspector] public Vector3 CARD_ROT;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Hit, S_GameFlowStateEnum.Store };

        CARD_ROT = new Vector3(0, 0, Random.Range(-LEAN_VALUE, LEAN_VALUE));

        obj_Card.transform.DOLocalRotate(CARD_ROT, 0);
    }
    public override void ForceExit()
    {
        base.ForceExit();

        obj_Card.transform.DOLocalRotate(CARD_ROT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
    }
}
