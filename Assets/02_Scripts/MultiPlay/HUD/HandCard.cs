using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // ������Ʈ
    PlayerController ownerPlayer;

    // UI ����
    public Card cardInfo;
    public CardEffect cardEffect;
    public PRS OriginPRS;
    const float POINTER_ENTER_ANIMATION_TIME = 0.3f;
    bool isSelected = false;
    bool isPointerEnter = false;

    // ī�� �ð� ȿ��
    [SerializeField] Image handCardEffectImage;
    [SerializeField] TMP_Text handCardNumber;
    [SerializeField] TMP_Text handCardEffectText1;
    [SerializeField] TMP_Text handCardEffectText2;
    [SerializeField] Image handCardSelectedEdge;

    void Update()
    {
        if (ownerPlayer.playerCards.IsDirectHitOrStandCycle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!isPointerEnter)
                {
                    isSelected = false;
                    handCardSelectedEdge.DOFade(0f, 0.2f).SetEase(Ease.OutQuart).SetAutoKill(true);
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.layer != LayerMask.NameToLayer("HitBtn"))
                    {
                        ownerPlayer.playerCards.CurrentSelectCard = default;
                    }
                }
            }
        }
    }
    public void SetCardInfo(Card card, PlayerController player)
    {
        // �÷��̾� �Ҵ�
        ownerPlayer = player;

        // ī�� ���� ����
        cardInfo = card;
        cardEffect = CardEffectList.FindCardEffectToKey(card.CardEffectKey.ToString());

        // ī�� �ؽ�Ʈ ����
        //handCardEffectImage = // ���߿� ��巹����� ī�� �̹��� �������� �� �׷��� ����
        handCardNumber.text = card.Number.ToString();
        handCardEffectText1.text = cardEffect.Name.ToString();
        handCardEffectText2.text = cardEffect.Name.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<RectTransform>().DOAnchorPos(OriginPRS.Pos + new Vector3(0, 15, 0), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        GetComponent<RectTransform>().DOScale(OriginPRS.Scale + new Vector3(0.5f, 0.5f, 0.5f), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        isPointerEnter = true;

        WorldUICardInfo.Instance.DisplayCardInfo(true, cardInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<RectTransform>().DOAnchorPos(OriginPRS.Pos, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        GetComponent<RectTransform>().DORotate(OriginPRS.Rot, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        GetComponent<RectTransform>().DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        isPointerEnter = false;

        WorldUICardInfo.Instance.DisplayCardInfo(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!ownerPlayer.playerCards.IsDirectHitOrStandCycle) return; // ���̷�Ʈ ��Ʈ�� �ƴ϶�� ���� ��� ����

        if (!isSelected)
        {
            isSelected = true;
            ownerPlayer.playerCards.CurrentSelectCard = cardInfo;
            handCardSelectedEdge.DOFade(1f, 0.2f).SetEase(Ease.OutQuart);
        }
        else
        {
            isSelected = false;
            ownerPlayer.playerCards.CurrentSelectCard = default;
            handCardSelectedEdge.DOFade(0f, 0.2f).SetEase(Ease.OutQuart);
        }
    }
}
