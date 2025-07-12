using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_DeckCardObj : S_CardObj
{
    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Deck, S_GameFlowStateEnum.Used };
    }
}