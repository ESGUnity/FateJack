using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.Netcode;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    [SerializeField] GameObject handCardPrefab;
    Transform handCardBase;
    PlayerController ownerPlayer;

    // 카드 연출 효과 관련
    List<GameObject> handCards = new();
    const float CARD_ANIM_TIME = 0.7f;
    Vector3 handLeftPos = new Vector3(-600f, 0, 0);
    Vector3 handRightPos = new Vector3(600f, 0, 0);
    Vector3 handLeftRot = new Vector3(0, 0, 12f);
    Vector3 handRightRot = new Vector3(0, 0, -12f);

    void Start()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        handCardBase = Array.Find(transforms, c => c.gameObject.name.Equals("HandCardBase"));

        GameFlowManager.Instance.InGameStart += InitHandManager;
    }

    void InitHandManager()
    {
        ownerPlayer = PlayerController.LocalInstance;
        ownerPlayer.playerCards.PlayerDeck.OnListChanged += SetPlayerDeck;
    }

    void SetPlayerDeck(NetworkListEvent<Card> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<Card>.EventType.Add:
                GameObject go = Instantiate(handCardPrefab, handCardBase);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -300, 0);
                go.GetComponent<HandCard>().SetCardInfo(changeEvent.Value, ownerPlayer);
                handCards.Add(go);
                AlignmentHandCard();
                break;
            case NetworkListEvent<Card>.EventType.Remove:
                foreach (GameObject obj in handCards)
                {
                    if (obj.GetComponent<HandCard>().cardInfo.Equals(changeEvent.Value))
                    {
                        handCards.Remove(obj);
                        Destroy(obj);
                        break;
                    }
                }
                AlignmentHandCard();
                break;
        }
    }

    void AlignmentHandCard()
    {
        List<PRS> originCardPRS = SetHandCardPos(handCards.Count);

        for (int i = 0; i < handCards.Count; i++)
        {
            handCards[i].GetComponent<HandCard>().OriginPRS = originCardPRS[i];
            handCards[i].GetComponent<RectTransform>().DOAnchorPos(originCardPRS[i].Pos, CARD_ANIM_TIME).SetEase(Ease.OutQuart);
            handCards[i].GetComponent<RectTransform>().DORotate(originCardPRS[i].Rot, CARD_ANIM_TIME).SetEase(Ease.OutQuart);
        }
    }

    List<PRS> SetHandCardPos(int cardCount)
    {
        float[] lerps = new float[cardCount];
        List<PRS> results = new List<PRS>(cardCount);

        switch (cardCount)
        {
            case 1:
                lerps = new float[] { 0.5f };
                break;
            case 2:
                lerps = new float[] { 0.40f, 0.60f };
                break;
            case 3:
                lerps = new float[] { 0.3f, 0.5f, 0.7f }; 
                break;
            default:
                float interval = 1f / (cardCount - 1);
                for (int i = 0; i < cardCount; i++)
                {
                    lerps[i] = interval * i;
                }
                break;
        }

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(handLeftPos, handRightPos, lerps[i]);
            Vector3 rot = Vector3.zero;
            Vector3 scale = Vector3.one;
            float height = 75f;

            if (cardCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow((lerps[i] - 0.5f) * height * 2, 2));
                pos.y += curve;
                rot = Vector3.Slerp(handLeftRot, handRightRot, lerps[i]);
            }
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }

    void OnDisable()
    {
        ownerPlayer.playerCards.PlayerDeck.OnListChanged -= SetPlayerDeck;
    }
}

public struct PRS
{
    public Vector3 Pos;
    public Vector3 Rot;
    public Vector3 Scale;

    public PRS(Vector3 pos, Vector3 rot, Vector3 scale)
    {
        Pos = pos;
        Rot = rot;
        Scale = scale;
    }
}