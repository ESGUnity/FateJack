using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_DeckCardObj : S_CardObj
{

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Deck };
    }

    #region VFX
    public override void UpdateCardState()
    {
        base.UpdateCardState();

        OnIsInDeckEffect();
    }
    public void OnIsInDeckEffect()
    {
        if (CardInfo.IsInDeck)
        {
            SetAlphaValue(1, 0);
        }
        else
        {
            SetAlphaValue(0.25f, 0);
        }
    }
    #endregion
}